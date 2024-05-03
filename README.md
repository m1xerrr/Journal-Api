# Forex Trading Journal API

Welcome to the Forex Trading Journal API documentation! This RESTful API, developed in C# using the Onion Architecture, aims to provide users with analytical insights into their trading accounts by optimizing data retrieved from external APIs of brokers and proprietary trading firms. The application also interacts with a database to securely store user information.

## Introduction
The Forex Trading Journal API serves as a backend system for managing trading account data and providing analytical insights to users. It connects with external APIs of brokers and proprietary trading firms to retrieve real-time trading data and optimizes this data for frontend interaction. The application is built on the Onion Architecture, ensuring a modular and maintainable codebase.

## Architecture Overview
The application architecture follows the principles of the Onion Architecture, which consists of layers such as Repositories, Services, Domain, and Controllers:
* __Domain Layer:__ This layer contains the declaration of response models, input models, and other data models. It serves as the foundation for defining the structure of data entities used throughout the application.
* __Repositories Layer:__ Repositories are responsible for fetching data from external sources, including the database for user information and external APIs for loading new trading data. These repositories encapsulate the data access logic, providing a clean separation between data retrieval and business logic.
* __Services Layer:__ Services operate on the data retrieved from repositories, normalizing and processing it to make it suitable for interaction with the frontend. These services implement the core business logic of the application, performing operations such as data normalization, analysis, and optimization.
* __Controllers Layer:__ Controllers receive requests from the frontend and invoke the appropriate services to handle these requests. They are responsible for handling the HTTP protocol, routing requests to the correct endpoints, and returning responses to the client.

## API Endpoints

* AddMTAccount
* DeleteTradingAccount
* AddCTraderAccount
* AddDXTradeAccount
* GetCTraderAccessToken
* TradingAccountData
* LoadTradingAccountData
* AddDealImg
* AddDealNote
* GetDeal
* Login
* Signup
* DeleteUser
* UserTradingAccounts
* ChangeUsername
* ChangeUserPassword
* ChangeUserEmail
* ChangeUserRole
* EditUser
* GetAllUsers
* GetLeaderboard
* ShareProfit

This list will be extended because the application is still developing.

## Configuration
The application configuration can be customized using the appsettings.json file. Here, you can specify settings such as database connection strings, API keys for external services, and logging configurations.
## Usage
Once the API is running, you can interact with it using HTTP requests. You can use tools such as cURL, Postman, or any programming language's HTTP client library to make requests to the API endpoints.
## Contributing
Contributions to the Forex Trading Journal API are welcome! If you'd like to contribute, please fork the repository, make your changes, and submit a pull request. Be sure to follow the contribution guidelines outlined in the repository.

Thank you for choosing the Forex Trading Journal API. If you have any questions or need further assistance, please don't hesitate to contact us.
