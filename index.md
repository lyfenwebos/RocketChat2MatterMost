# RocketChat to MatterMost Import

## Intro
This Program helps you migrate chat history from Rocket Chat by creating JSONL file for Mattermost.

## Requirements
For Program to work you need:
- Set variables accordingly to your setup (explained below);
- Be member of the Channel in RocketChat;
- Have Personal Access Token in RocketChat;
- Know how to make API requests;

### How does Program work

Program reads JSON file with RocketChat Channel's history, gets mandatory keys' values and creates [correct JSONL file](https://docs.mattermost.com/deployment/bulk-loading.html#post-object) for MatterMost.

After Import to MatterMost you will have:
- Full chat history;
- All replies are in correct places;
- Messages' timestamps are moved;
- Attachments(Files(any),  Images (jpeg/png)) are moved;
- Reactions(not all, you can add) are moved;
- Links in messages are moved;

## Getting RocketChat Channel's History through API
**Notice:** I used Postman WebUI with Postman Agent.
[RocketChat API Documentation 1](https://docs.rocket.chat/api/rest-api/methods/channels/history)
[RocketChat API Documentation 2](https://docs.rocket.chat/api/rest-api/methods/groups/history)

### Getting UserID
- Open Postman, create workspace and start a new request;
- Create POST Request with link `https://example.com/api/v1/login`, where example.com - RocketChat landing page;
- Set Body to Raw - JSON;
- Put JSON, Send.
`
{
    "user": "username",
    "password": "password",
    "code": "include this key if you have 2FA enabled in RocketChat with code from your Authenticator App"
}
`
- Find userId in response and write it down

### Getting GroupID
/api/v1/groups.list

### Getting chat history from RocketChat Channel
- Change Request type to GET;
- Set link `https://example.com/api/v1/groups.history`, where example.com - RocketChat landing page;
- Set Headers:
1. X-Auth-Token - Personal Access Token
2. X-User-Id - userId
- Set Parameters:
1. roomId - 
2. inclusive - true




## Preparing Variables
- RCJSON - Path to 
