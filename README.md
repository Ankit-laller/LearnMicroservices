# RabbitMQ Request–Response Pattern in .NET 8

This project demonstrates how to implement an **async Request–Response (RPC) communication** pattern between two microservices using **RabbitMQ** and **ASP.NET Core 8**.

---

## 📌 Scenario

* **BookingService** → sends a booking request (amount to be debited).
* **CreditService** → validates the agent’s credit limit, debits the balance, and replies with status + remaining balance.
* If credit check passes ✅ → booking is confirmed.
* If credit check fails ❌ → booking is rejected.

---

## 🏗️ Architecture

```
BookingService -------------------> RabbitMQ -------------------> CreditService
      (Producer / Client)                                 (Consumer / Server)
          |                                                      |
          |-------------- Reply Queue <---------------------------|
```

* `CorrelationId` → ensures the response matches the request.
* `ReplyTo` queue → temporary queue created for the client to receive the response.
* `BackgroundService` → runs CreditService consumer continuously on startup.

---

## 🔧 Tech Stack

* **.NET 8 (ASP.NET Core Web API)**
* **RabbitMQ.Client 7+ (async API)**
* **Entity Framework Core (optional for DB integration)**
* **Clean Architecture** (Controller → AppService → Repository)

---

## 📂 Project Structure

```
/BookingService
   └── Controllers
   └── Utils (RabbitMqHelper.cs)

/CreditService
   └── Controllers
   └── Services
   └── Utils (RabbitMqHelper.cs, CreditConsumerBackgroundService.cs)
```

---

## 🚀 Usage

### 1️⃣ Start RabbitMQ

Run RabbitMQ locally using Docker:

```bash
docker run -d --hostname rabbit --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
```

UI available at: [http://localhost:15672](http://localhost:15672) (user: `guest`, pass: `guest`).

---

### 2️⃣ Start **CreditService**

* On startup, it registers `CreditConsumerBackgroundService`.
* This service listens to `credit-requests` queue and replies with credit validation result.

---

### 3️⃣ Start **BookingService**

* Calls `RabbitMqHelper.SendRequestAsync<CreditRequestDto, CreditResponseDto>()`.
* Waits for a response (with timeout).
* If success → booking confirmed, else rejected.

---

## 📜 Sample DTOs

```csharp
public class CreditRequestDto
{
    public int UserId { get; set; }
    public decimal Amount { get; set; }
}

public class CreditResponseDto
{
    public bool Success { get; set; }
    public decimal AvailibleBalance { get; set; }
    public string Message { get; set; } = "";
}
```

---

## 🧑‍💻 Example API Flow

### Booking Request

```http
POST /api/booking/book
{
  "userId": 1,
  "amount": 500
}
```

### Possible Responses

✅ Success:

```json
{
  "message": "Booking confirmed",
  "balance": 2500
}
```

❌ Failure:

```json
{
  "success": false,
  "availibleBalance": 300,
  "message": "Insufficient balance"
}
```

---

## 🌟 Features

* Generic RabbitMQ helper methods:

  * `SendRequestAsync<TRequest, TResponse>`
  * `StartConsumerAsync<TRequest, TResponse>`
* Clean separation of concerns via **AppService + Repository**.
* Async/await support for scalability.
* Easily extendable to other services (Payment, Notification, etc).

---

## 📌 Next Steps

* Add **retry policy** for transient RabbitMQ failures.
* Configure **prefetchCount** for load balancing.
* Add **DLQ (Dead Letter Queue)** for failed messages.

---

## 🔗 GitHub Repo

👉 [Insert your repo link here]

---

## 📜 License

MIT License – free to use and modify.
