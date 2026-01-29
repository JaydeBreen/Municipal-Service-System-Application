# Municipal Service System Application
## Project Overview
This application provides a platform for citizens to report municipal service requests and view local announcements. The project demonstrates proficiency in **C# / .NET** development and the implementation of advanced **data structures and algorithms** to ensure efficiency and scalability.
---

## 1. Setup, Compiling, and Running Instructions

This section provides the comprehensive instructions required to set up and execute the application.

### ⚙️ Prerequisites
* **C# / .NET SDK:** Version 6.0 or higher is required.
* **Database:** A working connection to a data persistence layer (e.g., Firestore or local database).

### 🚀 Compiling and Execution

1.  **Clone the Repository:**
    Navigate to your desired directory and clone the project:
    git clone https://github.com/VCSTDN2024/prog7312-poe-JaydeBreen.git
    cd MunicipalServiceSystem
  

2.  **Restore Dependencies:**
    Ensure all necessary packages are installed:
    ```bash
    dotnet restore
    ```

3.  **Build the Project:**
    Compile the source code:
    ```bash
    dotnet build
    ```

4.  **Run the Application:**
    Start the application. It will typically open a browser window at `https://localhost:[Port]`.
    ```bash
    dotnet run
    ```

---

## 2. Program Usage and Feature Demonstration

### A. Main Navigation

The **Main Menu** provides organized options for accessing core features:

1.  **Report New Request:** (Leads to form submission)
2.  **View Service Status:** (Leads to the main tracker list)
3.  **Local Events & Announcements:** (Demonstrates Priority Queue, Stack, and Set)

### B. Core Data Structure Usage

The application uses **custom** data structures to ensure optimal performance for its key features:

| Feature/Data Role | Data Structure | Implementation Type | Efficiency | Purpose |
| :--- | :--- | :--- | :--- | :--- |
| **Status Update Lookup** | **Custom Hash Map** (`SimpleDictionary<K, V>`) | Custom C# Class | **O(1) average** | Maps unique Request IDs to their object data for near-instant status retrieval and updates. |
| **Event Prioritization** | **Custom Priority Queue** | Custom C# Class | **O(log n)** | Orders announcements (e.g., road closures) by urgency, ensuring critical alerts appear first. |
| **Feature History** | **Custom Stack** (`SimpleStack<T>`) | Custom C# Class | **O(1)** | Manages the history of recently viewed service requests or screens for quick navigation. |
| **Category Filtering** | **Set** (`HashSet<string>`) | Inbuilt C# Class | **O(1) average** | Generates a unique, non-redundant list of categories for filtering the events page. |

### C. Advanced Algorithmic Functionality (Service Matching)

To provide efficient service request matching and prevent duplicates, the application implements two key algorithms:

1.  **Merge Sort (O(n log n)):** Used to efficiently sort all existing service request titles as a pre-processing step.
2.  **Binary Search (O(log n)):** Performed on the sorted list to check if a new request title already exists, providing immediate, highly scalable feedback to the user.
```eof

