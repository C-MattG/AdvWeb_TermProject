# AdvWeb_TermProject
CSCI 3110 Advanced Web Semester Project - Highland Tech Solutions

AI Disclosure:    I swear and affirm I did not use AI tools to generate or complete this project.

I DO HAVE A DASHBOARD!!!
It is in the footer beside the Privacy Policy.  My nav bar was getting too busy.
It updates in real time as appointments are booked, quotes are requested, and services are changed

----------------------
-----Instructions-----
----------------------
1. Once project is downloaded and open, build the solution
2. Open the nuget PMC and run Add-Migration InitialCreate
3. Once the Migration is added, run Update-Database
4. Run the project


--------------------
-----How to Use-----
--------------------
There are three kinds of users on this website.  There are those who are not logged in, 
those who are logged in as a customer, and a single business owner who controls the website.
Nav options change depending on your user status

There are 4 user accounts (3 customers, 1 admin) seeded in Program.cs
There are also 12 services seeded (for users to schedule appointments for)
The business owner can CRRUD services, quote requests, appointments, and a few more.

-Admin User Login
  E-mail:          admin@highlandtech.com
  Password:        Graduation2025!

-Customer Login (2 more in Program.cs)
  E-mail:          john.public@mail.com
  Password:        GodspeedGoBucs2025!
