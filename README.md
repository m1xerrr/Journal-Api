
Forex Trading Journal API
Welcome to the Forex Trading Journal API documentation! This RESTful API, developed in C# using the Onion Architecture, aims to provide users with analytical insights into their trading accounts by optimizing data retrieved from external APIs of brokers and proprietary trading firms. The application also interacts with a database to securely store user information.
Table of Contents
Introduction
Architecture Overview
API Endpoints
Installation
Configuration
Usage
Contributing
Introduction
The Forex Trading Journal API serves as a backend system for managing trading account data and providing analytical insights to users. It connects with external APIs of brokers and proprietary trading firms to retrieve real-time trading data and optimizes this data for frontend interaction. The application is built on the Onion Architecture, ensuring a modular and maintainable codebase.
Architecture Overview
The application architecture follows the principles of the Onion Architecture, which consists of layers such as Repositories, Services, Domain, and Controllers:
Domain Layer: This layer contains the declaration of response models, input models, and other data models. It serves as the foundation for defining the structure of data entities used throughout the application.
Repositories Layer: Repositories are responsible for fetching data from external sources, including the database for user information and external APIs for loading new trading data. These repositories encapsulate the data access logic, providing a clean separation between data retrieval and business logic.
Services Layer: Services operate on the data retrieved from repositories, normalizing and processing it to make it suitable for interaction with the frontend. These services implement the core business logic of the application, performing operations such as data normalization, analysis, and optimization.
Controllers Layer: Controllers receive requests from the frontend and invoke the appropriate services to handle these requests. They are responsible for handling the HTTP protocol, routing requests to the correct endpoints, and returning responses to the client.
API Endpoints
AccountsController
POST /api/accounts/AddMTAccount
Description: Add a MetaTrader account.
Request Body:
json
Copy code
{
 "username": "string",
 "password": "string",
 "server": "string",
 "userId": "string"
}


Response: Returns a JSON object with the status of the operation.
POST /api/accounts/DeleteTradingAccount
Description: Delete a trading account based on the provider type.
Request Body:
json
Copy code
{
 "accountId": "string",
 "provider": "string"
}


Response: Returns a JSON object with the status of the operation.
POST /api/accounts/AddCTraderAccount
Description: Add a CTrader account.
Request Body:
json
Copy code
{
 "accessToken": "string",
 "userId": "string"
}


Response: Returns a JSON object with the status of the operation.
POST /api/accounts/AddDXTradeAccount
Description: Add a DXTrade account.
Request Body:
json
Copy code
{
 "username": "string",
 "password": "string",
 "domain": "string",
 "userId": "string"
}


Response: Returns a JSON object with the status of the operation.
POST /api/accounts/GetCTraderAccessToken
Description: Get access token for CTrader account.
Request Body:
json
Copy code
"authorizationLink": "string"


Response: Returns a JSON object with the access token.
UserAccountsController
POST /api/useraccounts/TradingAccountData
Description: Get trading account data based on the provider type.
Request Body:
json
Copy code
{
 "accountId": "string",
 "provider": "string"
}


Response: Returns a JSON object with the trading account data.
POST /api/useraccounts/LoadTradingAccountData
Description: Load trading account data based on the provider type.
Request Body:
json
Copy code
{
 "accountId": "string",
 "provider": "string"
}


Response: Returns a JSON object with the status of the operation.
POST /api/useraccounts/AddDealImg
Description: Add image to a deal.
Request Body:
json
Copy code
{
 "Id": "string",
 "accountId": "string",
 "Field": "string"
}


Response: Returns a JSON object with the status of the operation.
POST /api/useraccounts/AddDealNote
Description: Add note to a deal.
Request Body:
json
Copy code
{
 "Id": "string",
 "accountId": "string",
 "Field": "string"
}


Response: Returns a JSON object with the status of the operation.
POST /api/useraccounts/GetDeal
Description: Get deal details.
Request Body:
json
Copy code
{
 "Id": "string",
 "accountId": "string"
}


Response: Returns a JSON object with the deal details.
UserController
POST /api/user/Login
Description: User login.
Request Body:
json
Copy code
{
 "username": "string",
 "password": "string"
}


Response: Returns a JSON object with the status of the login operation.
POST /api/user/Signup
Description: User signup.
Request Body:
json
Copy code
{
 "username": "string",
 "password": "string",
 "email": "string"
}


Response: Returns a JSON object with the status of the signup operation.
POST /api/user/DeleteUser
Description: Delete a user account.
Request Body:
json
Copy code
"userId": "string"


Response: Returns a JSON object with the status of the operation.
POST /api/user/UserTradingAccounts
Description: Get trading accounts associated with a user.
Request Body:
json
Copy code
"userId": "string"


Response: Returns a JSON object with the trading account details.
POST /api/user/ChangeUsername
Description: Change username.
Request Body:
json
Copy code
{
 "userId": "string",
 "newUsername": "string"
}


Response: Returns a JSON object with the status of the operation.
POST /api/user/ChangeUserPassword
Description: Change user password.
Request Body:
json
Copy code
{
 "userId": "string",
 "newPassword": "string"
}


Response: Returns a JSON object with the status of the operation.
POST /api/user/ChangeUserEmail
Description: Change user email.
Request Body:
json
Copy code
{
 "userId": "string",
 "newEmail": "string"
}


Response: Returns a JSON object with the status of the operation.
POST /api/user/ChangeUserRole
Description: Change user role.
Request Body:
json
Copy code
{
 "userId": "string",
 "newRole": "string"
}


Response: Returns a JSON object with the status of the operation.
POST /api/user/EditUser
Description: Edit user details.
Request Body:
json
Copy code
{
 "userId": "string",
 "newUsername": "string",
 "newEmail": "string",
 "newPassword": "string",
 "newRole": "string"
}


Response: Returns a JSON object with the status of the operation.
POST /api/user/GetAllUsers
Description: Get details of all users.
Request Body: None
Response: Returns a JSON object with details of all users.
POST /api/user/DeleteUserSubscription
Description: Delete user subscription.
Request Body:
json
Copy code
"id": "string"


Response: Returns a JSON object with the status of the operation.
POST /api/user/GetLeaderboard
Description: Get leaderboard.
Request Body: None
Response: Returns a JSON object with leaderboard data.
POST /api/user/ShareProfit
Description: Share profit.
Request Body:
json
Copy code
{
 "accountId": "string",
 "provider": "string",
 "startDate": "string",
 "endDate": "string"
}


Response: Returns a JSON object with profit sharing details.
These endpoints allow users to perform various operations related to managing their accounts, accessing trading data, and interacting with the platform.
Installation
To install and run the Forex Trading Journal API locally, follow these steps:
Clone the repository from GitHub:
bash
Copy code
git clone https://github.com/your/repository.git


Navigate to the project directory:
bash
Copy code
cd forex-trading-journal-api


Install dependencies using NuGet:
Copy code
dotnet restore


Build the project:
Copy code
dotnet build


Run the application:
arduino
Copy code
dotnet run


The API should now be running locally and accessible at http://localhost:5000.
Configuration
The application configuration can be customized using the appsettings.json file. Here, you can specify settings such as database connection strings, API keys for external services, and logging configurations.
Usage
Once the API is running, you can interact with it using HTTP requests. You can use tools such as cURL, Postman, or any programming language's HTTP client library to make requests to the API endpoints.
For example, to retrieve a list of trading accounts, you can send a GET request to the /api/accounts endpoint:
bash
Copy code
GET http://localhost:5000/api/accounts


Contributing
Contributions to the Forex Trading Journal API are welcome! If you'd like to contribute, please fork the repository, make your changes, and submit a pull request. Be sure to follow the contribution guidelines outlined in the repository.

Thank you for choosing the Forex Trading Journal API. If you have any questions or need further assistance, please don't hesitate to contact us.

