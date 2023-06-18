# User management microservice
## Main responsibilities
1. Storing user accounts data, such as name, phone number, password etc.
2. Providing API for creating, editing and deleting accounts and getting an accounts data.

## Data format
#### User data
```json
{
  "id": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", 
  "email": "some_email123@gl.net", 
  "password": "newpassword123", 
  "name": "Some Name", 
  "phone": "+380981231234",
  "registration_date": "2022-01-01T00:00:00"
}
```

## API
### Register new user
```javascript
POST api/users/register/
```
##### Input
```json
{
  "id": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "name": "Some Name",
  "email": "some_email123@gl.net",
  "password": "newpassword123",
  "phone": "+380981231234"
}
```
##### Output
```
Status code 2XX if registration successful or 4XX otherwise
```

### Get all users
```javascript
GET api/users/
```
##### Output
```json
{
  "count": 1,
  "users": [
    {
      "id": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
      "name": "Some Name"
    }
  ]
}
```

### Get user by id
```javascript
GET api/users/{id}
```
##### Output
```json
{
  "id": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "email": "some_email123@gl.net",
  "name": "Some Name",
  "phone": "+380981231234",
  "registration_date": "2022-01-01T00:00:00"
}
```

### Update user
```javascript
PUT api/users/
```
##### Input
```json
{
  "id": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "name": "Some Name",
  "email": "some_email123@gl.net",
  "password": "newpassword123",
  "phone": "+380981231234"
}
```
##### Output
```
Status code 2XX if user successfully updated or 4XX otherwise
```

### Delete user
```javascript
DELETE api/users/{id}
```
##### Output
```
Status code 2XX if user successfully deleted or 4XX otherwise
```
