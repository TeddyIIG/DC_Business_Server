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
            catch (FaultException ex) //Catches FaultException that is being thrown from the server side
            {
                Console.WriteLine(ex.Message); //Logs the exception in the console for debugging purposes
                throw new FaultException(ex.Message); //rethrows the fault exception towards the client
            
            }
                          
        }

        [MethodImpl (MethodImplOptions.Synchronized)] //Thread locking
        public bool SearchandSelect(uint cusid) //This method searches the list of users and returns a boolean according to the existence of the relevant user
        {
            try
            {

                if (data_server.GetUsers().Contains(cusid)) //Checks if the list of users contain a user with the relevant customer id
                {

                    data_server.SelectUser(cusid); //The user with the relevant customer id is selected for this call
                    return true; //If the user is found

                }
                else
                {

                    return false; //If the user is not found
                }
            }
            catch(FaultException ex) //Catches FaultException that is being thrown from the server side
            {
                Console.WriteLine(ex.Message); //Logs the exception in the console for debugging purposes
                throw new FaultException(ex.Message); //rethrows the fault exception towards the client
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
            catch(FaultException ex) //Catches FaultException that is being thrown from the server side
            {
                Console.WriteLine(ex.Message); //Logs the exception in the console for debugging purposes
                throw new FaultException(ex.Message); //rethrows the fault exception towards the client
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
            catch (FaultException ex) //Catches FaultException that is being thrown from the server side
            {
                Console.WriteLine(ex.Message); //Logs the exception in the console for debugging purposes
                throw new FaultException(ex.Message); //rethrows the fault exception towards the client
            }


        }


/** -------------------------------------------------- END OF USER BASED FUNCTIONS ------------------------------------------------------ **/

/** -------------------------------------------------- ACCOUNT BASED FUNCTIONS ---------------------------------------------------------- **/


        [MethodImpl(MethodImplOptions.Synchronized)] //Making atomic functions thread safe so that no two clients access this method at the same time (in case one user is accessed via two clients)
        public uint CreateAccount(uint UserID) //creates the account based on the current user id and returns an account ID
        {
            try
            {
                uint AccID = data_server.CreateAccount(UserID); //Creates the user account in the bank server and retrieves the account ID
                data_server.SaveToDisk(); //Saves the changed data to the users.json file at C:/WebStuff/
                return AccID; //return the account ID of the created user account
            }
            catch(FaultException ex) //Catches FaultException that is being thrown from the server side
            {
                Console.WriteLine(ex.Message); //Logs the exception in the console for debugging purposes
                throw new FaultException(ex.Message); //rethrows the fault exception towards the client
            }
                
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public List<uint> ListAccountsbyId(uint UserID) //Basically returns a list of accounts associated with a particular user ID
        {
            try
            {
                List<uint> acclist = data_server.GetAccountIDsByUser(UserID); //assigns the returned list a list variable called acclist
                return acclist; //returns the list when the function is called
            }
            catch (FaultException ex) //Catches FaultException that is being thrown from the server side
            {
                Console.WriteLine(ex.Message); //Logs the exception in the console for debugging purposes
                throw new FaultException(ex.Message); //rethrows the fault exception towards the client
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
            catch (FaultException ex) //Catches FaultException that is being thrown from the server side
            {
                Console.WriteLine(ex.Message); //Logs the exception in the console for debugging purposes
                throw new FaultException(ex.Message); //rethrows the fault exception towards the client
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
            catch (FaultException ex) //Catches FaultException that is being thrown from the server side
            {
                Console.WriteLine(ex.Message); //Logs the exception in the console for debugging purposes
                throw new FaultException(ex.Message); //rethrows the fault exception towards the client


            }
            

        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool Withdraw(uint accid, uint withdraw_amount) //returns a boolean status on wether the withdraw was successful or not
        {
            try
            {
                data_server.SelectAccount(accid); //Selects the user account with the relevant account ID
                if (data_server.GetBalance() >= withdraw_amount) //Checks if the selected account has enough balance to withdraw the specified amount
                {
                    data_server.Withdraw(withdraw_amount); //Withdraws the amount in the bank server from the selected user account
                    return true; //If successfully withdrawn
                }

                else
                {
                    return false; //If withdrawal was not possible
                }
            }
            catch(FaultException ex) //Catches FaultException that is being thrown from the server side
            {
                Console.WriteLine(ex.Message); //Logs the exception in the console for debugging purposes
                throw new FaultException(ex.Message); //rethrows the fault exception towards the client
            }
            

        }

        /** ----------------------------------------------------------- END OF ACCOUNT FUNCTIONS ------------------------- **/

        /** ----------------------------------------------------------- START OF TRANSACTION FUNCTION ------------------------- **/

        [MethodImpl(MethodImplOptions.Synchronized)]
        public List<uint> DisplayTransactions (uint accID) //This method returns the list of transactions belonging to the account of the relevant account ID
        {
            try
            {
                data_server.SelectAccount(accID); //Selects the user account with the relevant account ID
                List<uint> Transactionlist = data_server.GetTransactions(); //Retrieves the list of transcations from the bank server for the selected user account
                return Transactionlist; //returns the transaction list
            }
            catch (FaultException ex) //Catches FaultException that is being thrown from the server side
            {
                Console.WriteLine(ex.Message); //Logs the exception in the console for debugging purposes
                throw new FaultException(ex.Message); //rethrows the fault exception towards the client
            }

        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public uint CreateTransaction(uint accID) //Creates a new transaction and returns the transaction ID
        {
            try
            {
                data_server.SelectAccount(accID); //Selects the user account with the relevant account ID
                uint transID = data_server.CreateTransaction(); //Creates
                return transID;
            }
            catch (FaultException ex) //Catches FaultException that is being thrown from the server side
            {
                Console.WriteLine(ex.Message); //Logs the exception in the console for debugging purposes
                throw new FaultException(ex.Message); //rethrows the fault exception towards the client
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool setTransaction(uint sendID, uint receivID, uint amount,uint transID) //This method sets the details of the transaction and returns a boolean depending on the success
        {
            try
            {
                data_server.SelectTransaction(transID); //Selects the transaction in the bank server

                if (amount <= this.GetBalance(sendID)) //Checks if the user account balance is suffeicient to perform the transaction
                {
                    data_server.SetRecvr(receivID); //Sets the reciever ID for the selected transaction
                    data_server.SetSendr(sendID); //Sets the sender ID for the selected transaction
                    data_server.SetAmount(amount); //Sets the amount for the selected transaction
                    return true;
                }
                else
                {
                    return false; //if the amount is insufficient
                }
            }
            catch(FaultException ex) //Catches FaultException that is being thrown from the server side
            {
                Console.WriteLine(ex.Message); //Logs the exception in the console for debugging purposes
                throw new FaultException(ex.Message); //rethrows the fault exception towards the client
            }

        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ShowTransactionDetail(out uint sendID, out uint receivID, out uint amount, uint transID) //This method outputs the account ID's of the sender and receiver and the amount of the relevant transaction
        {
            try
            {
                data_server.SelectTransaction(transID); //The relevant transaction is selected in the bank server
                amount = data_server.GetAmount(); //acquires the amount of the relevant transaction
                receivID = data_server.GetRecvrAcct(); //acquires the receiver's account ID
                sendID = data_server.GetSendrAcct(); //acquires the sender's account ID
            }
            catch(FaultException ex) //Catches FaultException that is being thrown from the server side
            {
                Console.WriteLine(ex.Message); //Logs the exception in the console for debugging purposes
                throw new FaultException(ex.Message); //rethrows the fault exception towards the client
            }
            
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void commitalltransactions() //This method saves all the transactions to the user.json file at C:/WebStuff/
        {
            try
            {
                data_server.ProcessAllTransactions(); //Processes all the transactions in the bank server before saving
                data_server.SaveToDisk(); //Saves the changed data to the users.json file at C:/WebStuff/
            }
            catch(FaultException ex) //Catches FaultException that is being thrown from the server side
            {
                Console.WriteLine(ex.Message); //Logs the exception in the console for debugging purposes
                throw new FaultException(ex.Message); //rethrows the fault exception towards the client
            }
        }
    }
}
