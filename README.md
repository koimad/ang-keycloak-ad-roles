This demo solution was put together using a windows machine using docker for windows to run the required keycloak container, and windows hyper-v manager is used for hosting a domain controller required for a ldap connection within keycloak to be created. 

In the Aspirations realm you will need to update it to use your windows ldap service, plus change the roles mapper named groups to have the correct settings for populating the groups from windows.

To get key cloak up and running use the below command, together with saved keycloak database is in the keycloak folder

docker run -d --restart always --name keycloak -v c/temp/keycloak:/opt/keycloak/data -p 8080:8080 -e KC_BOOTSTRAP_ADMIN_USERNAME=admin -e KC_BOOTSTRAP_ADMIN_PASSWORD=admin quay.io/keycloak/keycloak:26.0.5 start-dev

The Visual studio solution containers a angular application hosted from AngApp.Server and a Angular Client application angapp.client

The API endpoints for Model1, Model2,Model3, and Model4 are hosted in ExternalServices

To run the solution the ExternalServices, and AngApp.Server  should be the startup projects for the solution.


