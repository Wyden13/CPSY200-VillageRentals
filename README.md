# Village Rentals Management System - CPSY200 Final Project

## Project Overview

This project is a desktop application for **Village Rentals**, a family-run equipment rental business. It was developed as the final project for CPSY200, demonstrating a full software development lifecycle from analysis and design (Part A & B) to a functional prototype (Part C).

The application is built using **.NET MAUI Blazor** and connects to a **SQL Server** database to manage customers, equipment inventory, and rental transactions. It allows employees to perform all necessary daily operations and generate key business reports.

---

## Database Setup Instructions

To run this application, you must first create and populate the database. Please follow these steps carefully.

### 1. Create the Database and Tables

* Open **SQL Server Management Studio (SSMS)**.
* Create a new database. You can name it `VillageRentalsDB`.
* Open the `SSMS.txt` file included in the project.
* Copy the entire SQL script from this file.
* Paste the script into a new query window in SSMS, ensure you are targeting your new `VillageRentalsDB` database, and execute the script. This will create all the necessary tables (`Customers`, `Equipment`, `Categories`, `Rentals`, `RentalItems`).

### 2. Populate the Tables with Sample Data

* Open the `SSMS - Data.txt` file included in the project.
* Copy the entire SQL script from this file.
* Paste the script into a new query window in SSMS and execute it. This will populate the tables with the initial sample data required for testing.

After these steps, the database will be ready for the application.

---

## Use Case Implementation (Part C Prototype)

This prototype demonstrates the core functionality of the system. Each major use case is handled by a specific page in the application.

### Use Case: Manage Customer Information

* **Page:** `CustomerPage.razor`
* **Functionality:** This page fulfills the requirement to manage customer information.
    * **Display:** It loads and displays a list of all customers from the database.
    * **Add:** A new customer can be added using the form on this page.
    * **Update:** An existing customer's details can be edited and saved.
    * **Delete:** A customer can be removed from the system.

### Use Case: Manage Equipment Inventory

* **Page:** `EquipmentPage.razor`
* **Functionality:** This page handles the management of the equipment inventory.
    * **Display:** It shows a complete list of all equipment, including details like category and availability.
    * **Add:** New equipment can be added to the inventory.
    * **Update & Delete:** Existing equipment can be edited or removed.

### Use Case: Process Equipment Rental

* **Page:** `ProcessRental.razor`
* **Functionality:** This is the core transactional page for processing a new rental.
    * The clerk can search for and select a customer.
    * The clerk can search for available equipment and add multiple items to the rental.
    * The system calculates the total cost.
    * Upon finalizing, the rental and its associated items are saved to the database, and the equipment's availability is updated.

### Use Case: Generate Reports

* **Page:** `ReporstPage.razor`
* **Functionality:** This page allows for the generation of key business reports.
    * The user can select from three report types: Sales by Date, Sales by Customer, and List Items by Category.
    * Based on the selection, the user can provide filters (e.g., date range).
    * The system queries the database and displays the generated report in a table.

### How to Run the Application

1.  Ensure the database is set up and the connection string is configured correctly.
2.  Open the `VillageRentalManagementSystem.sln` file in Visual Studio.
3.  Set the startup project to `VillageRentalManagementSystem`.
4.  Run the application (press F5 or the Start button). The home page will appear, and you can navigate to the different pages using the menu on the left.