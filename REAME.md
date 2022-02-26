## Deploy to Heroku
1. Create Dockerfile
1. Create a new Heroku app: nam-home-assistant
2. heroku container:push web -a <app_name>
heroku container:push web -a nam-home-assistant
3. heroku container:release web -a <app_name>
heroku container:release web -a nam-home-assistant