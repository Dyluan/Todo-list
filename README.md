# Todo List Application

A full-stack Todo List application built with a React frontend and a .NET backend, designed to manage and track tasks effectively.

## Features

- Add, edit, delete, and toggle the completion status of tasks.
- Backend API with persistent storage.
- Frontend with a dynamic and user-friendly interface.

## Project Structure

### Frontend (React)

- Implements the task management UI.
- Interacts with the backend via API calls for CRUD operations.

### Backend (.NET)

- Provides RESTful APIs to manage tasks.
- Includes data persistence using a JSON file.

---

## Getting Started

### Prerequisites

- [Node.js](https://nodejs.org/) (for the frontend)
- [.NET SDK](https://dotnet.microsoft.com/download) (for the backend)

---

## Installation

### 1. Clone the Repository

```bash
git clone https://github.com/Dyluan/Todo-list.git
cd Todo-list
```

### 2. Set Up Backend

1. Navigate to the backend directory:
   ```bash
   cd api
   ```
2. Restore dependencies and build the project:
   ```bash
   dotnet restore
   dotnet build
   ```
3. Start the backend server:
   ```bash
   dotnet run
   ```
By default, the backend will run on http://localhost:5000.

### 3. Set Up Frontend

Navigate to the frontend directory:
   ```bash
   cd ../frontend
   ```
   Install dependencies:
   ```bash
   npm install
   ```
   Start the frontend server:
   ```bash
   npm start
   ```
By default, the frontend will run on http://localhost:3000.

---

## API Endpoints

Base URL: http://localhost:5000

| Method | Endpoint | Description |
| ------ | -------- | ----------- |
| GET | /todos | Fetch all todos |
| GET | /todos/{id} | Fetch a specific todo by id |
| POST | /todos | Add a new todo |
| PUT | /todos/{id} | Update a todo's completion state |
| DELETE | /todos/{id} | Delete a todo by id |

---

## How to Use

Navigate to http://localhost:3000 in your browser.
Add a new task using the input field and the "+" button.
Click on a task to toggle its completion state.
Use the "X" button next to a task to delete it.

## Technical details 

### Frontend Highlights

    Framework: React
    Key Dependencies:
        uuid for generating unique IDs.
        axios for API requests (if applicable).

### Backend Highlights

    Framework: .NET Core
    Data Storage: JSON file for persistence.
    CORS Enabled: Allows the React app to connect to the backend.
