# RocketChat to MatterMost Import

**Notice**: RocketChat has Channel and Group, but their difference is in privacy, I'll be referencing them both as a "Channel"  
**Notice**: You are doing this at your own risk. I am not responsible for any consequences.

## Intro
This Program helps you migrate chat history from Rocket Chat by creating JSONL file for Mattermost.


## Requirements
For Program to work you need:
- Set variables accordingly to your setup (explained below);
- Be member of the Channel in RocketChat;
- Have Personal Access Token in RocketChat;
- Have the same users in target MatterMost Channel as in source RocketChat Channel;
- Have access to the MatterMost CLI;

### How does Program work

Program reads JSON file with RocketChat Channel's history, gets mandatory keys' values and creates [correct JSONL file](https://docs.mattermost.com/deployment/bulk-loading.html#post-object) for MatterMost.

After Import to MatterMost you will have:
- Full chat history;
- All replies are in correct places;
- Messages' timestamps are moved;
- Attachments(Files(any),  Images (jpeg/png)) are moved;
- Reactions(not all, you can add) are moved;
- Links in messages are moved;

## Migrating RocketChat Channel's History to MatterMost Channel
#### Useful Links
[MatterMost Bulk Import Documentation](https://docs.mattermost.com/deployment/bulk-loading.html)  
[RocketChat API Documentation: Authentication](https://docs.rocket.chat/api/rest-api/methods/authentication/login)  
[RocketChat API Documentation: Groups List](https://docs.rocket.chat/api/rest-api/methods/groups/list)  
[RocketChat API Documentation: Channels History](https://docs.rocket.chat/api/rest-api/methods/channels/history)  
[RocketChat API Documentation: Groups History](https://docs.rocket.chat/api/rest-api/methods/groups/history)  

#### Definitions in terms of RocketChat
Group - a private group;  
Channel - a public channel;

**Notice:** I used Postman WebUI with Postman Agent.
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
- Create GET request with link `https://example.com/api/v1/groups.history` or `https://example.com/api/v1/channels.history`, where example.com - RocketChat landing page;
- Set Headers:
1. X-Auth-Token - Personal Access Token
2. X-User-Id - userId
- Find Channel you want to export and get its roomId

### Getting chat history from RocketChat Channel
- Create GET request with link `https://example.com/api/v1/groups.history` or `https://example.com/api/v1/channels.history`, where example.com - RocketChat landing page;
- Set Headers:
1. X-Auth-Token - Personal Access Token
2. X-User-Id - userId
- Set Parameters:
1. roomId - roomId got in previous step;
2. inclusive - include first and last message from the retrieved list;
3. count - set amount of messages to retrieve (0=all)
- Copy response to a file. Make sure it is in JSON;

### Getting RocketChat Cookies
- Login to webUI RocketChat;
- Press F12> Application> Cookies> RocketChat URL;
- Write down rc_uid and rc_token;

### Preparing Variables
- RCJSON - Path to a response file in JSON with Channel's history;
- rchome - Rocket chat landing page (e.g. `https://example.com`);
- outputdir - Directory where JSONL and attachments will be placed during Programm run. These are to used for import;
- importdir - Location of directory for all contents from outputdir on mattermost server;
- teamname - Team name in MatterMost where channel is located;
- channelname - Channel name in which history will be imported;
- rcuid - rc_uid Cookie;
- rctoken - rc_token Cookie;  

After you finish with variables, Run Programm

### Importing Chat History into MatterMost Channel
- Copy ***all contents*** of **outputdir** in a directory you mentioned in **importdir**
- Go to root directory of mattermost (default `/opt/mattermost`)
**Notice**: Personally suggesting to disable Bleeve(System Console>Experimental>Bleeve>false), stop MatterMost, do config backup (`cp /opt/mattermost/config/config.json /opt/mattermost/config/config.json.bck`) and do server backup.
- Run validation command  
`sudo -u mattermost bin/mattermost import bulk /path/to/data.jsonl --validate`  
If successful, then you can apply it
- Run apply command  
`sudo -u mattermost bin/mattermost import bulk /path/to/data.jsonl --apply`  
If no errors reported, start MatterMost, Go to System Console > Web Server > Purge All Caches, restart application.  
- Check Channel history.

## TODO
- Change the way reactions are selected. Needs something better.
- Reactions array between MatterMost and RocketChat Emojis.
