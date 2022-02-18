# Bookery

**Bookery** is a multiplatform app that in general stands for a cloud drive. It uses hierarchical storage, supports basic operations like upload/download/rename/delete, and supports sharing. It is possible to share and hide nodes (folders or files) recursively.

This project contains microservices that handle all app activities. All of them are containerized with `Docker Compose` but some use `Azure` services, make sure you have a valid subscription if you decide to run the project.

Also, there is a [`Xamarin`](https://github.com/VengerAndrey/BookeryMobile) client app for this project.

<p align="center">
<img src="https://github.com/VengerAndrey/Bookery/blob/master/Images/infrastructure.png">
</p>

## Tech stack

All microservices run on **.NET Core 6.0**.

### Bookery.API
* **Ocelot** as a gateway
* **JWT** validation

### Bookery.Authentication
* **JWT** issuing, refresh, sign out
* **Azure Tables** as identity source

### Bookery.Node
* **MSSQL Server** in container

### Bookery.Storage
* **Azure Blobs**

### Bookery.User
* **MSSQL Server** in container
* **Azure Tables** for providing new identity

### Message Broker
* **RabbitMQ** in container 

### Deployment
* **Docker Compose**

## Installation

<details><summary><b>Linux</b></summary>

1. Verify installation of `Docker` and `Docker Compose`:
    ```bash
    docker version && docker compose version
    ```
    
2. Clone the repository:
    ```bash
    git clone https://github.com/VengerAndrey/Bookery.git
    ```

3. Navigate inside the repository directory:
    ```bash
    cd Bookery
    ```
    
4. Replace Azure Storage connection string in `appsettings.json` files in `Bookery.Node`, `Bookery.User` and `Bookery.Storage`.

5. Start microservices with the command:
    ```bash
    docker compose up -d
    ```

6. Wait a few seconds after containers are created and then access API at [`localhost:5100`](http://localhost:5100/).


7. To clean up run:
    ```bash
    docker compose down -v --rmi all --remove-orphans
    ```

</details>

## Troubleshooting

### Docker Compose
Make sure every container is up by running the following command in the root directory:
```bash
    docker compose ps
```
If some of the services have a state different from `running` you should run them manually:
```bash
    docker compose up <service> -d
```

### Docker daemon
If you use Linux and get Docker daemon error similar to the following:
```bash
Got permission denied while trying to connect to the Docker daemon socket at unix
/var/run/docker.sock: connect: permission denied
```

Consider running the command:
```bash
sudo chmod 666 /var/run/docker.sock
```