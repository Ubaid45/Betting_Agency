# API Description
Swagger is implemented for the documentation of the API. The authentication and authorization are implemented by JSON web token. Furthermore, the application can be tested via Postman.

The API has 3 endpoints
- Get The Token
- Get All of users
- Place the bet

## Pre-populated Data
When the application is started, the in-memory database is created and the data is seeded. To test the application, one can proceed with the following username and password combination.
```javascript
username: 'ubaid45', password: 'Ubaid',
userName = 'mario12', password = 'Mario',
userName = 'alex34', password = 'Alex'
```
## GetToken
Enter the username and password to generate a token. This token will be needed to access the further endpoints to authorize the user.

## GetList
This endpoint returns all the populated users in the database. It is secured by auth token which can be generated in the **GetToken** endpoint.

## PlaceBet
This endpoint is also secured by auth token which can be generated in the **GetToken** endpoint. This token will help to identify the player and his previous score. Once the user make his bet, system generate a random number between 0-9. if the guessed number is matched with the randomly generated number on server, user will win the bet and his stake will be multiplied by 9 and added that into his previous balance. A lost bet will result in the deduction of stake * 9 from current balance. An extra field **LuckyNumber** in the response shows what was the winning number which was randomly generated on the server.
