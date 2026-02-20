# 💈 BerberOto - Barber Shop SaaS

**BerberOto** is a modern SaaS solution designed for barber shops to manage appointments, customers, and daily operations efficiently. Built with **.NET 8** and **SQLite**, it offers a lightweight yet powerful platform for small to medium-sized businesses.

![License](https://img.shields.io/badge/license-MIT-blue.svg)
![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)
![Status](https://img.shields.io/badge/status-active-success.svg)

## ✨ Features

- **📅 Appointment Management:** Easy-to-use interface for booking and managing appointments.
- **👥 Customer Management:** Keep track of customer history and preferences.
- **admin Admin Panel:** Comprehensive dashboard for shop owners to manage settings and view reports.
- **🔔 SMS Notifications:** Integrated SMS service (supports Netgsm) for appointment reminders and confirmations.
- **📱 Responsive Design:** Optimized for both desktop and mobile devices.
- **🤖 Background Services:** Automated tasks for appointment reminders.

## 🛠️ Tech Stack

- **Framework:** [.NET 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) (ASP.NET Core MVC)
- **Database:** SQLite (Entity Framework Core)
- **Frontend:** HTML5, CSS3, JavaScript, Bootstrap (or custom CSS)
- **Services:** Background Hosted Services, Distributed Memory Cache

## 🚀 Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Any code editor (VS Code, Visual Studio 2022 recommended)

### Installation

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/EnesKaraca44/BerberOto.git
    cd BerberOto
    ```

2.  **Restore dependencies:**
    ```bash
    dotnet restore
    ```

3.  **Run the application:**
    ```bash
    dotnet run
    ```
    The application will automatically create the SQLite database (`BerberOto.db`) and seed initial data if configured.

4.  **Access the app:**
    Open your browser and navigate to `http://localhost:5000` (or the port specified in the console).

## ⚙️ Configuration

Start by checking `appsettings.json` for configuration settings.

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

To enable real SMS sending, you may need to configure the `ISmsService` implementation in `Program.cs` to use `NetgsmSmsService` instead of `LogSmsService`.

## 📸 Screenshots

*(Add screenshots of your application here to show off the UI!)*

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1.  Fork the project
2.  Create your feature branch (`git checkout -b feature/AmazingFeature`)
3.  Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4.  Push to the branch (`git push origin feature/AmazingFeature`)
5.  Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
