using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ServerUserInterface;
using System.Threading;


namespace BankBusinessServer
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false, InstanceContextMode = InstanceContextMode.Single)]
    class BusinessUserImplementation 
    {

        private uint UserID;
        private uint AccID;
        private uint TransactionID;
        private static readonly Object threadlock = new Object();



        public BusinessUserImplementation()
        {
            bankfuncobj = new BankFunctionality();
            userobj = bankfuncobj.IUserAccess();

        }


        public uint CreateUser(string fname, string lname)  //This function creates a user by accessing the returned user interface
        {
            //  try
            //    {
            // lock (threadlock)
            //   {
            UserID = userobj.CreateUser();   //this creates a user and assigns a userid
            userobj.SetUserName(fname, lname);  //This will set the first name and last name entered by the user from the client side
            Console.WriteLine(UserID); //for debugging purposes
            bankfuncobj.Savetodisk(); //Writing the created user to the users.json file at C:/WebStuff/
            return UserID; //returning the userid to the client
                           //   }
                           //      }
                           /**       catch (FormatException ex)
                                  {

                                      return 0;
                                  }
                                  catch (ArgumentNullException ex2)
                                  {

                                      return 1;
                                  } **/

        }

        public void SelectUser(uint cusid)
        {
            lock (threadlock)
            {
                userobj.SelectUser(cusid);
                UserID = cusid;
                Thread.Sleep(1500);

            }

        }

        public uint EditUser(string fname, string lname) //This function allows the client side to edit the user first name and last name 
        {
            //  try
            //   {
            //         lock (threadlock)
            //       {
            userobj.SelectUser(UserID); //Selects the user 
            userobj.SetUserName(fname, lname); //Calls the function from BankDB.dll to set the user first names and last name, please note that the user must be selected for this to happen
            bankfuncobj.Savetodisk(); //Saves the changed data to the users.json file at C:/WebStuff/
            return 3;
            //        }
            //   }
            //   catch (FormatException ex)
            //   {

            //         return 0;
            //     }
            //    catch (ArgumentNullException ex2)
            //{
            //
            //    return 1;
            //   }
        }
        /**
                public List<uint> userList() //Basically returns a list of userids for already created users
                {
                    try
                    {
                        return userobj.GetUsers();
                    }
                    catch (NullReferenceException ex)
                    {
                        return new List<uint>();
                    }

                } **/


        public void getuser(out string fname, out string lname) //This will return the user first name and last name after a user has been selected 
        {

            userobj.SelectUser(UserID);
            userobj.GetUserName(out fname, out lname);


        }
        /**
                public uint CreateAccount() //creates the account based on the current user id and returns
                {
                    try
                    {
                        lock (threadlock)
                        {
                            AccID = accobj.CreateAccount(UserID);
                            bankfuncobj.Savetodisk();
                            return AccID;
                        }
                    }
                    catch (NullReferenceException ex)
                    {

                        return 0;
                    }


                }

                public List <uint> ListAccountsbyId ()
                {
                    try
                    {
                        lock (threadlock)
                        {
                            List<uint> acclist = accobj.GetAccountIDsByUser(UserID);
                            return acclist;
                        }
                    }
                    catch(ArgumentNullException ex)
                    {
                        return new List<uint>();
                    }
                }

                public uint GetBalance()
                {
                    try
                    {
                        lock (threadlock)
                        {
                            accobj.SelectAccount(AccID);
                            uint balance = accobj.GetBalance();
                            return balance;
                        }
                    }
                    catch (ArgumentNullException ex)
                    {
                        return 0;
                    }
                }

                public void Deposit(uint accid, uint dep_amount)
                {
                    accobj.SelectAccount(accid);
                    accobj.Deposit(dep_amount);
                    this.flushnsave();

                }

                public Boolean Withdraw(uint accid, uint withdraw_amount)
                {
                    accobj.SelectAccount(accid);
                    if (accobj.GetBalance() > withdraw_amount)
                    {

                        accobj.Withdraw(withdraw_amount);
                        this.flushnsave();
                        return true;
                        

                    }

                    else
                    {
                        return false;
                    }

                }
            
                private void flushnsave() //This will complete any pending transactions and flushes all data to the disk. Possibly used at the end of 
                {
                    bankfuncobj.CommitAllTransactions();
                    bankfuncobj.Savetodisk();

                }**/
    }
}
