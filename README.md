# PeerCraft

PeerCraft is a web application built for our Web Application Development course project. The goal is to provide a simple platform for students to manage their group projects. It allows users to create projects, track tasks, add team members, and discuss progress in one place, simulating a real-world collaborative tool.

---

## Features

* **User Accounts:** Users can sign up for an account and log in. Passwords are kept secure using BCrypt hashing.

* **Project Creation & Management:** Logged-in users can create new projects, providing a title, description, and an optional GitHub link. The person who creates the project is assigned as the owner.

* **Team Collaboration:** Project owners can add other registered users to their projects by username. They can also remove members. All tasks assigned to a removed member become unassigned.

* **Task Tracking:** Within a project, team members can create tasks, assign them to other members, and set optional due dates. The completion status of each task can be toggled.

* **Dashboard:** Each user has a personal dashboard that shows their overdue and upcoming tasks, projects they own, and projects they are a team member of. It also includes a live feed of recent activity (new comments, completed tasks) from their projects.

* **Project Discovery:** A public "Discover" page lists all projects on the platform. This page includes a feature to filter projects by the technologies they use (e.g., C#, React, Python).

* **Feedback and Discussion:** Every project has its own discussion board where team members can post comments and reply to each other, allowing for feedback and conversation.

---

## Technology Used

* **Framework:** ASP.NET Core 3.1 MVC
* **Data Access:** Entity Framework Core 3.1
* **Database:** SQL Server (designed for LocalDB)
* **Authentication:** ASP.NET Core Cookie Authentication
* **Frontend:** Bootstrap 5 for styling and layout.

---

## How to Run the Project

To set up and run this project on a local machine, follow these steps.

1.  **Prerequisites:**
    * You need the .NET Core 3.1 SDK installed.
    * You need an instance of SQL Server running (like LocalDB, which is standard with Visual Studio).

2.  **Clone the Repository:**
    ```bash
    git clone [https://github.com/dhruvalpatel-dev/PeerCraft.git](https://github.com/dhruvalpatel-dev/PeerCraft.git)
    cd PeerCraft/PeerCraft
    ```

3.  **Setup the Database:**
    * Check the connection string in `appsettings.json` to make sure it matches your local SQL Server setup.
    * Run the database migrations using the Entity Framework Core tools. This will create the database and its tables.
    ```bash
    dotnet ef database update
    ```

4.  **Run the Application:**
    * Use the `dotnet run` command to start the web server.
    ```bash
    dotnet run
    ```
    * The application should now be accessible in your browser at `https://localhost:5001`.

---

## Team Members & Contributions

* **Dhruval Patel**
    * Responsible for the backend development. This included setting up the database models and DbContext with Entity Framework, building the user authentication system (registration, login, and password hashing), and writing the core C# logic in the controllers for managing projects, tasks, comments, and team members. Also developed the dashboard's data aggregation logic.

* **Rudra Dave**
    * Responsible for the frontend development and UI/UX. This included creating the main site layout, all Razor Views (`.cshtml` files), and styling the application with Bootstrap 5 and custom CSS. Handled the creation of ViewModels to pass data from the controllers to the views and implemented the Discover page with its filtering functionality.
