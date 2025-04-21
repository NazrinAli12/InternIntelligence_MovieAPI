
 🎬 Movie API Backend - InternIntelligence Task 3

This is a fully functional ASP.NET Core Web API for managing and searching movie data, developed as part of the **Intern Intelligence** backend internship program (Task 3).

---

 🔥 Features

- 🔍 **Search Movies** using TMDB (The Movie Database) API  
- 🎞️ **Get Movie Details** by movie ID  
- 🗂️ **Get Popular Movies** from TMDB  
- 📦 **Clean architecture** with service, interface, and controller layers  
- 🚫 **404 Handling** for no exact matches  
- 🛡️ **Input validation** and error handling

---

 🌐 External Integration

- **TMDB API** is used for fetching real-time movie data.
- You must provide your TMDB API key in the configuration.

---

 🚀 Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET    | `/api/Movie/search?query=movieName` | Search for a movie by exact name |
| GET    | `/api/Movie/popular` | Get popular movies from TMDB |
| GET    | `/api/Movie/details/{id}` | Get full details for a specific movie |
| GET    | `/api/Movie` | Get all locally stored movies |
| GET    | `/api/Movie/{id}` | Get movie by ID from local DB |
| POST   | `/api/Movie` | Add a new movie to local DB |
| PUT    | `/api/Movie/{id}` | Update movie details |
| PUT    | `/api/Movie/soft-delete/{id}` | Soft delete a movie |
| PUT    | `/api/Movie/restore/{id}` | Restore a soft-deleted movie |
| DELETE | `/api/Movie/hard-delete/{id}` | Hard delete a movie permanently |

---

 ⚙️ Technologies Used

- ASP.NET Core Web API (C#)
- Entity Framework Core
- AutoMapper
- Swagger / Postman
- JSON Serialization
- TMDB REST API Integration

---

 🛠️ Configuration

In your `appsettings.json`, set your TMDB API key:

```json
"TMDB": {
  "ApiKey": "your_tmdb_api_key_here"
}
```

You can get your TMDB API key from: [https://www.themoviedb.org/settings/api](https://www.themoviedb.org/settings/api)

---

 🧪 Testing

You can test endpoints using:
- Swagger UI: `https://localhost:{port}/swagger`
- Postman or any HTTP client

Example request:

```
GET https://localhost:{port}/api/Movie/search?query=The Gorge
```

---

 📂 Project Structure

```
├── Controllers
│   └── MovieController.cs
├── Services
│   └── MovieApiService.cs
├── Interfaces
│   └── IMovieApiService.cs
├── Models
├── DTOs
├── Data
│   └── AppDbContext.cs
├── appsettings.json
└── Program.cs / Startup.cs
```

---

 🙌 Author

Created with ❤️ by **Nazrin Aliyeva** for Intern Intelligence Task 3.

[![LinkedIn](https://img.shields.io/badge/LinkedIn-blue?style=flat&logo=linkedin)](https://www.linkedin.com/in/nazrin-aliyeva-b141b0327/)
[![GitHub](https://img.shields.io/badge/GitHub-black?style=flat&logo=github)](https://github.com/NazrinAli12)

---

📌 Notes

- Only exact match search is supported to keep results clean.
- If no exact match is found, a `404 Not Found` response is returned.
- Popular movies and genre information are pulled directly from TMDB API.

---

 ✅ Task Completed

- ☑️ Task 3 - Movie Data Backend  
- ☑️ Successfully integrated third-party API (TMDB)  
- ☑️ Applied clean code and architecture practices  

---

 📬 Contact  

Feel free to reach out via [LinkedIn](https://www.linkedin.com/in/nazrin-aliyeva-b141b0327) for collaboration or feedback.
