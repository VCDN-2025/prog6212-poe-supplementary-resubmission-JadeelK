# Contract Monthly Claim System (CMCS)  
Programming 2B – PROG6212 
Final Project – Part 3

A web-based ASP.NET Core MVC application for managing monthly contract claims at a university.  
Lecturers submit claims with supporting documents, Coordinators verify them, Managers give final approval, and HR handles reporting + user management.

# Features
- Lecturer: Submit claims with auto-calculation of total amount, input validation, file uploads (PDF/DOCX/XLSX – encrypted)
- Coordinator: View pending claims, verify/approve/reject with comments
- Manager: Final approval/rejection on verified claims
- HR: Dashboard with statistics, full claim reports, add/edit/delete lecturers
- In-memory data storage (no database required)
- Simple role-based access via session (demo login)
- Modern Bootstrap 5 UI with responsive design & icons
- File encryption for supporting documents
- Client & server-side validation

# Technologies Used
- ASP.NET Core 8.0 (MVC)
- C# 12
- Bootstrap 5 + Bootstrap Icons
- In-memory singleton services
- AES file encryption
- Session-based role simulation

# How to Run
1. Open solution in Visual Studio 2022
2. Ensure .NET 8.0 SDK is installed
3. Build → Run (HTTPS)
4. On home page → select role (Lecturer / Coordinator / Manager / HR)
5. Test full workflow: submit → verify → approve → HR report
   

### Commits Summary
See commit history for detailed changes.

# YouTube Link :  https://youtu.be/diWSk1cxw2I

Student: Jadeel Kisten 
Student Number: ST10339718 
Date: 12 January 2025
