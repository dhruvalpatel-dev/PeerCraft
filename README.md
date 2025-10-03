# PeerCraft

A web platform for students to manage, showcase, and get feedback on academic and personal projects. Built with ASP.NET Core MVC, PeerCraft provides a collaborative space to track progress, assign tasks, and discuss ideas.

---

## Core Features

* **User Authentication:** Secure user registration and login system using cookie-based authentication and password hashing.
* **Project Management:** Users can create, edit, and delete their own projects, providing details like title, description, and a GitHub link.
* **Team Collaboration:** Project owners can add or remove other users as team members to their projects.
* **Task Tracking:** Create, assign, and track tasks within each project. The dashboard highlights overdue and upcoming tasks for the logged-in user.
* **Project Discovery:** A "Discover" page where users can browse all public projects and filter them by the technologies used.
* **Feedback System:** A complete discussion section on each project page for comments and nested replies.
* **Dynamic Dashboard:** A personalized dashboard for each user showing their owned projects, team projects, and a recent activity feed from all their projects.

---

## Tech Stack

* **Framework:** ASP.NET Core 3.1 (MVC)
* **Database:** Entity Framework Core with SQL Server (LocalDB)
* **Authentication:** ASP.NET Core Cookie Authentication
* **Password Hashing:** BCrypt.Net-Next
* **Frontend:** Bootstrap 5, HTML, CSS, JavaScript

---

## Getting Started

To run this project locally, follow these steps:

1.  **Clone the repository:**
    ```bash
    git clone [https://github.com/dhruvalpatel-dev/PeerCraft.git](https://github.com/dhruvalpatel-dev/PeerCraft.git)
    cd PeerCraft/PeerCraft
    ```

2.  **Prerequisites:**
    * Ensure you have the [.NET Core 3.1 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/3.1) installed.
    * Ensure you have SQL Server (Express or LocalDB) installed.

3.  **Update Database Connection:**
    * Open `appsettings.json` and verify the `DefaultConnection` connection string points to your SQL Server instance. The default is set to use `(localdb)\mssqllocaldb`.

4.  **Run Database Migrations:**
    * Open a terminal in the project directory (`PeerCraft/PeerCraft`) and run the following command to create and seed the database:
    ```bash
    dotnet ef database update
    ```

5.  **Run the application:**
    ```bash
    dotnet run
    ```
    The application will be running at `https://localhost:5001`.

---

## Team Contributions

This project was a collaborative effort by:

* **Dhruval Patel** (`dhruvalpatel-dev`)
    * **Responsibilities:** Backend Architecture & Core Logic.
    * Designed and implemented the database schema using Entity Framework Core.
    * Developed the complete user authentication system (registration, login, password hashing).
    * Implemented the core backend logic for project, task, and team member management in the `ProjectController`.
    * Built the dynamic user dashboard logic in the `HomeController`.
    * Implemented the comment and reply system backend.

* **Rudra Dave** (`rudradave490-design`)
    * **Responsibilities:** UI/UX & Frontend Development.
    * Designed the overall site layout, navigation, and user interface using `_Layout.cshtml` and Bootstrap 5.
    * Created the UI and ViewModels for the Project Creation, Details, and Edit pages.
    * Implemented the "Discover" page functionality, including the controller and view for filtering projects.
    * Handled all CSS styling and static asset management.
    * Developed the UI for the user profile page.
