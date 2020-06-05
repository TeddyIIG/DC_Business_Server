using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ServerUserInterface;
using System.Threading;
using BankBusinessInterface;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BankBusinessServer
{
    [ServiceBehavior(IncludeExceptionDetailInFaults =true, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false, InstanceContextMode = InstanceContextMode.PerCall)]
    class BusinessUserImplementation : IBankBusinessInterface
    {
        ConnectionClass con_object = new ConnectionClass();
        UserInterface data_server;
        



        public BusinessUserImplementation()
        {
            data_server = con_object.ConnectedInterface();
                       
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Startup()
        {
            Console.WriteLine("Client Connecting");
        }

        /** ---------------------------------- USER BASED FUNCTIONS ----------------------------------------------- **/

        [MethodImpl(MethodImplOptions.Synchronized)]
        public uint CreateUser(string fname, string lname)  //This function creates a user by accessing the returned user interface
        {

            try
            {
                uint UserID = data_server.CreateUser();   //this creates a user and assigns a userid
                data_server.SelectUser(UserID);
                data_server.SetUserName(fname, lname);
                Console.WriteLine("Created New User with ID : " + UserID); //for debugging purposes
                data_server.SaveToDisk(); //Writing the created user to the users.json file at C:/WebStuff/
                return UserID; //returning the userid to the client
            }
            catch (FaultException ex)
            {
                Console.WriteLine(ex.Message);
                throw new FaultException(ex.Message);
            
            }
                          
        }

        [MethodImpl (MethodImplOptions.Synchronized)] //Thread locking
        public bool SearchandSelect(uint cusid)
        {
            try
            {

                if (data_server.GetUsers().Contains(cusid))
                {

                    data_server.SelectUser(cusid);
                    return true;

                }
                else
                {

                    return false;
                }
            }
            catch(FaultException ex)
            {
                Console.WriteLine(ex.Message);
                throw new FaultException(ex.Message);
            }
               


        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void EditUser(string fname, string lname, uint UserID) //This function allows the client side to edit the user first name and last name 
        {
            try
            {
                data_server.SelectUser(UserID); //Selects the user 
                data_server.SetUserName(fname, lname); //Calls the function from BankDB.dll to set the user first names and last name, please note that the user must be selected for this to happen
                data_server.SaveToDisk(); //Saves the changed data to the users.json file at C:/WebStuff/
            }
            catch(FaultException ex)
            {
                Console.WriteLine(ex.Message);
                throw new FaultException(ex.Message);
            }
           
        }


        [MethodImpl(MethodImplOptions.Synchronized)] //Making atomic functions thread safe so that no two clients access this method at the same time (in case one user is accessed via two clients)
        public void getuser(out string fname, out string lname, uint UserID) //This will return the user first name and last name after a user has been selected 
        {
            try
            {

                data_server.SelectUser(UserID);
                data_server.GetUserName(out fname, out lname);
            }
            catch (FaultException ex)
            {
                Console.WriteLine(ex.Message);
                throw new FaultException(ex.Message);
            }


        }


/** -------------------------------------------------- END OF USER BASED FUNCTIONS ------------------------------------------------------ **/

/** -------------------------------------------------- ACCOUNT BASED FUNCTIONS ---------------------------------------------------------- **/


        [MethodImpl(MethodImplOptions.Synchronized)] //Making atomic functions thread safe so that no two clients access this method at the same time (in case one user is accessed via two clients)
        public uint CreateAccount(uint UserID) //creates the account based on the current user id and returns an account ID
        {
            try
            {
                uint AccID = data_server.CreateAccount(UserID);
                data_server.SaveToDisk();
                return AccID;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new FaultException(ex.Message);
            }
                
        }

        public List<uint> ListAccountsbyId(uint UserID) //Basically returns a list of accounts associated with a particular user ID
        {
            try
            {
                List<uint> acclist = data_server.GetAccountIDsByUser(UserID); //assigns the returned list a list variable called acclist
                return acclist; //returns the list when the function is called
            }
            catch (FaultException ex)
            {
                Console.WriteLine(ex.Message);
                throw new FaultException(ex.Message);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public uint GetBalance(uint AccID)  //returns the balance of a given account
        {
            try
            {
                data_server.SelectAccount(AccID);   //selects the account on which the balance needs to be retrieved
                uint balance = data_server.GetBalance(); //assigns the balance to a uint variable named balance
                return balance; //returns the balance varia
               
            }
            catch (FaultException ex)
            {
                Console.WriteLine(ex.Message);
                throw new FaultException(ex.Message);
            }
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Deposit(uint accid, uint dep_amount) //selects the specified account and deposits the amount
        {
            try
            {
                data_server.SelectAccount(accid); //selects the account in which the money needs to be deposited
                data_server.Deposit(dep_amount);    //calls the deposit function from the bank server
                
            }
            catch (FaultException ex)
            {
                Console.WriteLine(ex.Message);
                throw new FaultException(ex.Message);
                                
               
            }
            

        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool Withdraw(uint accid, uint withdraw_amount) //returns a boolean status on wether the withdraw was successful or not
        {
            try
            {
                data_server.SelectAccount(accid);
                if (data_server.GetBalance() > withdraw_amount)
                {

                    data_server.Withdraw(withdraw_amount);
                    return true;


                }

                else
                {
                    return false;
                }
            }
            catch(FaultException ex)
            {
                Console.WriteLine(ex.Message);
                throw new FaultException(ex.Message);
            }
            

        }

        /** ----------------------------------------------------------- END OF ACCOUNT FUNCTIONS ------------------------- **/

        /** ----------------------------------------------------------- START OF TRANSACTION FUNCTION ------------------------- **/

        [MethodImpl(MethodImplOptions.Synchronized)]
        public List<uint> DisplayTransactions (uint accID)
        {
            data_server.SelectAccount(accID);
            List<uint> Transactionlist  = data_server.GetTransactions();
            return Transactionlist;

        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public uint CreateTransaction(uint accID)
        {
            data_server.SelectAccount(accID);
            uint transID = data_server.CreateTransaction();
            return transID;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool setTransaction(uint sendID, uint receivID, uint amount,uint transID)
        {
            try
            {
                data_server.SelectTransaction(transID);

                if (amount <= this.GetBalance(sendID))
                {
                    data_server.SetRecvr(receivID);
                    data_server.SetSendr(sendID);
                    data_server.SetAmount(amount);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(FaultException ex)
            {
                Console.WriteLine(ex.Message);
                throw new FaultException(ex.Message);
            }

        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ShowTransactionDetail(out uint sendID, out uint receivID, out uint amount, uint transID)
        {
            try
            {
                data_server.SelectTransaction(transID);
                amount = data_server.GetAmount();
                receivID = data_server.GetRecvrAcct();
                sendID = data_server.GetSendrAcct();
            }
            catch(FaultException ex)
            {
                Console.WriteLine(ex.Message);
                throw new FaultException(ex.Message);
            }
            
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void commitalltransactions()
        {
            try
            {
                data_server.ProcessAllTransactions();
                data_server.SaveToDisk();
            }
            catch(FaultException ex)
            {
                Console.WriteLine(ex.Message);
                throw new FaultException(ex.Message);
            }
        }
        

        
    }
}
