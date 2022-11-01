# JWT Project with .net6

## To Run the Project

- Clone the repo and open in Visual studio.
- Add and empty Database and Update Connection string in **appsetting.json**
- run the command
  ```bash
  Update-Database
  ```
  The migrations added will handle everything.
- Launch the project and run the API path SeedData/Seed Database in Swagger UI to place initial data in db
- All set you're ready to log in, default user and password are **admin** for the admin role.

  ## When needing to authurize

- Log in with the admin account amd copy the jwt token provided.
- Click the authorize button and type "bearer " + past in the token you coppied.
- Congratulations you're logged in and got admin access.
