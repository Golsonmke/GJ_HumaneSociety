﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query
    {
        static HumaneSocietyDataContext db;

        static Query()
        {
            db = new HumaneSocietyDataContext();
        }

        internal static List<USState> GetStates()
        {
            List<USState> allStates = db.USStates.ToList();

            return allStates;
        }

        internal static Client GetClient(string userName, string password)
        {
            Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).Single();

            return client;
        }

        internal static List<Client> GetClients()
        {
            List<Client> allClients = db.Clients.ToList();

            return allClients;
        }

        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {
            Client newClient = new Client();

            newClient.FirstName = firstName;
            newClient.LastName = lastName;
            newClient.UserName = username;
            newClient.Password = password;
            newClient.Email = email;

            Address addressFromDb = db.Addresses.Where(a => a.AddressLine1 == streetAddress && a.Zipcode == zipCode && a.USStateId == stateId).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (addressFromDb == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = streetAddress;
                newAddress.City = null;
                newAddress.USStateId = stateId;
                newAddress.Zipcode = zipCode;

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                addressFromDb = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            newClient.AddressId = addressFromDb.AddressId;

            db.Clients.InsertOnSubmit(newClient);

            db.SubmitChanges();
        }

        internal static void UpdateClient(Client clientWithUpdates)
        {
            // find corresponding Client from Db
            Client clientFromDb = null;

            try
            {
                clientFromDb = db.Clients.Where(c => c.ClientId == clientWithUpdates.ClientId).Single();
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("No clients have a ClientId that matches the Client passed in.");
                Console.WriteLine("No update have been made.");
                return;
            }

            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            clientFromDb.FirstName = clientWithUpdates.FirstName;
            clientFromDb.LastName = clientWithUpdates.LastName;
            clientFromDb.UserName = clientWithUpdates.UserName;
            clientFromDb.Password = clientWithUpdates.Password;
            clientFromDb.Email = clientWithUpdates.Email;

            // get address object from clientWithUpdates
            Address clientAddress = clientWithUpdates.Address;

            // look for existing Address in Db (null will be returned if the address isn't already in the Db
            Address updatedAddress = db.Addresses.Where(a => a.AddressLine1 == clientAddress.AddressLine1 && a.USStateId == clientAddress.USStateId && a.Zipcode == clientAddress.Zipcode).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (updatedAddress == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = clientAddress.AddressLine1;
                newAddress.City = null;
                newAddress.USStateId = clientAddress.USStateId;
                newAddress.Zipcode = clientAddress.Zipcode;

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                updatedAddress = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            clientFromDb.AddressId = updatedAddress.AddressId;

            // submit changes
            db.SubmitChanges();
        }

        internal static void AddUsernameAndPassword(Employee employee)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if (employeeFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return employeeFromDb;
            }
        }

        internal static Employee EmployeeLogin(string userName, string password)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).FirstOrDefault();

            return employeeFromDb;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();

            return employeeWithUserName != null;
        }


        //// TODO Items: ////

        // TODO: Allow any of the CRUD operations to occur here
        internal static void RunEmployeeQueries(Employee employee, string crudOperation)
        {
            switch (crudOperation)
            {
                case "create":
                    db.Employees.InsertOnSubmit(employee);
                    break;
                case "read":
                    var employeeRead = db.Employees.FirstOrDefault(e => employee.EmployeeNumber == employee.EmployeeNumber);
                    Console.ReadLine();
                    break;
                case "update":
                    var employeeToUpdate = db.Employees.FirstOrDefault(e => employee.EmployeeNumber == employee.EmployeeNumber);
                    employeeToUpdate.FirstName = employee.FirstName;
                    employeeToUpdate.LastName = employee.LastName;
                    employeeToUpdate.Email = employee.Email;
                    db.SubmitChanges();
                    break;
                case "delete":
                    var employeeToDelete = db.Employees.FirstOrDefault(e => employee.EmployeeNumber == employee.EmployeeNumber);
                    db.Employees.DeleteOnSubmit(employee);
                    break;
                default:
                    Console.WriteLine("Please choose valid option. ");
                    RunEmployeeQueries(employee, crudOperation);
                    break;
            }
        }

        // TODO: Animal CRUD Operations
        internal static void AddAnimal(Animal animal)
        {
            db.Animals.InsertOnSubmit(animal);

            db.SubmitChanges();
        }

        internal static Animal GetAnimalByID(int id)
        {
            var searchAnimalId = db.Animals.Where(a => a.AnimalId == id).Single();
            if (searchAnimalId == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return searchAnimalId;
            }
           

        }

        internal static void UpdateAnimal(int animalId, Dictionary<int, string> updates)
        {
            Animal updateAnimal = db.Animals.Where(a => a.AnimalId == animalId).Single();
            foreach (KeyValuePair<int, string> animal in updates)
            {

                GetAnimalByID(animalId);
                    db.SubmitChanges();
                
            }


        }

        internal static void RemoveAnimal(Animal animal)
        {
            db.Animals.DeleteOnSubmit(animal);
            db.SubmitChanges();
        }

        // TODO: Animal Multi-Trait Search
        internal static IQueryable<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> updates) // parameter(s)? Query animal list options from customer menu
        {
            IQueryable<Animal> queryAnimals = db.Animals;

            foreach (KeyValuePair<int, string> keyValue in updates)
            {

                switch (keyValue.Key)
                {
                    case 1:
                        queryAnimals = queryAnimals.Where(q => q.CategoryId == GetCategoryId(keyValue.Value));
                        break;
                    case 2:
                        queryAnimals = queryAnimals.Where(q => q.Name == keyValue.Value);
                        break;
                    case 3:
                        queryAnimals = queryAnimals.Where(q => q.Age == int.Parse(keyValue.Value));
                        break;
                    case 4:
                        queryAnimals = queryAnimals.Where(q => q.Demeanor == keyValue.Value);
                        break;
                    case 5:
                        queryAnimals = queryAnimals.Where(q => q.KidFriendly == bool.Parse(keyValue.Value));
                        break;
                    case 6:
                        queryAnimals = queryAnimals.Where(q => q.PetFriendly == bool.Parse(keyValue.Value));
                        break;
                    case 7:
                        queryAnimals = queryAnimals.Where(q => q.Weight == int.Parse(keyValue.Value));
                        break;
                                   
                   
                }
               
            }
            return queryAnimals;
        }

        // TODO: Misc Animal Things
        internal static int GetCategoryId(string categoryName) //Where,Select
        {
            var categoryId = db.Categories.Where(c => c.Name == categoryName).Select(c => c.CategoryId).Single();
            return categoryId;
        }

        internal static Room GetRoom(int animalId) //
        {
            var searchRoom = db.Rooms.Where(r => r.RoomNumber == animalId).Single();
            return searchRoom;

        }

        internal static int GetDietPlanId(string dietPlanName)
        {
            var searchDiet = db.DietPlans.Where(d => d.DietPlanId == int.Parse(dietPlanName));
            return int.Parse(dietPlanName);
        }

        // TODO: Adoption CRUD Operations
        internal static void Adopt(Animal animal, Client client)
        {
            Adoption adoption = new Adoption();
            
            adoption.ClientId = client.ClientId;
            adoption.AnimalId = animal.AnimalId;
            adoption.ApprovalStatus = "Pending";
            adoption.AdoptionFee = 75;
            adoption.PaymentCollected = true;
            db.Adoptions.InsertOnSubmit(adoption);
            db.SubmitChanges();
        }
        internal static IQueryable<Adoption> GetPendingAdoptions()
        {
            IQueryable<Adoption> pendingAdoptions;
            pendingAdoptions = db.Adoptions.Where(a => a.ApprovalStatus == "Pending");
            return pendingAdoptions;
        }
        
        internal static void UpdateAdoption(bool isAdopted, Adoption adoption) 
        {
            //  isAdopted ? adoption.ApprovalStatus = "Adopted" : adoption.ApprovalStatus = "Available";
            if (isAdopted == true)
            {
                adoption.ApprovalStatus = "Adopted";
            }
            else
            {
                adoption.ApprovalStatus = "Available";
            }
        }

        internal static void RemoveAdoption(int animalId, int clientId)
        {
           var adoptions = db.Adoptions.Where(a => a.AnimalId == animalId &&  a.ClientId == clientId).SingleOrDefault();
            db.Adoptions.DeleteOnSubmit(adoptions);
            db.SubmitChanges();
        }

        // TODO: Shots Stuff
        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
            IQueryable<AnimalShot> animalShots = db.AnimalShots;
            animalShots = db.AnimalShots.Where(a => a.AnimalId == animal.AnimalId);
            return animalShots;
        }

        internal static void UpdateShot(string shotName, Animal animal)
        {
            AnimalShot animalShot = new AnimalShot();
            animalShot.AnimalId = animal.AnimalId;

              var  shotGave = db.Shots.Where(s => s.Name == shotName).SingleOrDefault();
            animalShot.ShotId = shotGave.ShotId;
            animalShot.DateReceived = DateTime.Now;
            db.AnimalShots.Where(c => c.AnimalId == shotGave.ShotId);
            db.AnimalShots.InsertOnSubmit(animalShot);
            db.SubmitChanges();
        }
    }
}