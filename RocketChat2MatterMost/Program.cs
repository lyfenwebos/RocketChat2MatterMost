using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RocketChat2MatterMost
{
    class Program
    {
        public static JArray reactionsArray = new JArray();
        static void Main(string[] args)
        {
            int replyCurr;
            string postID;
            dynamic versionRoot = new JObject();
            versionRoot.type = "version";
            versionRoot.version = 1;
            
            var RCJSON = JObject.Parse(File.ReadAllText("<Full Path to RocketChat JSON>"));
            var rchome = "<Link to RocketChat>";
            var outputdir = "<Output Directory for Application>";
            var importdir = "<Directory where attachments and jsonl will be put on MatterMost Server>";
            var importjsonl = outputdir + "data.jsonl";
            var teamname = "<TeamName in MatterMost>";
            var channelname = "<ChannelName in Team above>";
            var rcuid = "<rc_uid cookie from the browser after successful log in>";
            var rctoken = "<rc_token cookie from the browser after successful log in>";
            var postMax = ((JArray)RCJSON["messages"]).Count - 1;

            if (Directory.Exists(outputdir))
            {
                Directory.Delete(outputdir, true);
            }
            Directory.CreateDirectory(outputdir);
            File.WriteAllText(importjsonl, versionRoot.ToString(Formatting.None)+Environment.NewLine);

            for (int j = postMax; j >= 0; j--)
            {
                dynamic postRoot = new JObject();
                if ((RCJSON["messages"][j].HasValues == true) && (RCJSON["messages"][j]["u"]["username"].ToString() != "rocket.cat"))
                {
                    postRoot.type = "post";
                    var SimpleDateTime = Convert.ToDateTime(RCJSON["messages"][j]["ts"]);
                    var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    var unixDateTime = (SimpleDateTime.ToUniversalTime() - epoch).TotalMilliseconds;
                    var replyArray = new JArray();

                    var postObject = JObject.FromObject(new
                    {
                        post = new
                        {
                            team = teamname,
                            channel = channelname,
                            user = RCJSON["messages"][j]["u"]["username"],
                            message = RCJSON["messages"][j]["msg"],
                            props = JObject.FromObject(new
                            {
                            }),
                            create_at = Convert.ToInt64(unixDateTime)
                        }
                    });
                    postRoot.Merge(postObject);
                    postRoot["post"].Add("flagged_by", new JArray());

                    if (RCJSON["messages"][j]["replies"] != null)
                    {
                        var replyMax = Convert.ToInt32(RCJSON["messages"][j]["tcount"]);
                        replyCurr = 0;
                        postID = RCJSON["messages"][j]["_id"].ToString();

                        for (int postCurr = j - 1; postCurr >= 0; postCurr--)
                        {
                            if ((RCJSON["messages"][postCurr].HasValues) && (RCJSON["messages"][postCurr]["tmid"] != null))
                            {
                                if (RCJSON["messages"][postCurr]["tmid"].ToString() == postID)
                                {
                                    var ReplySimpleDateTime = Convert.ToDateTime(RCJSON["messages"][postCurr]["ts"]);
                                    var ReplyUnixDateTime = (ReplySimpleDateTime.ToUniversalTime() - epoch).TotalMilliseconds;

                                    JObject replyObject = JObject.FromObject(new
                                    {
                                        user = RCJSON["messages"][postCurr]["u"]["username"].ToString(),
                                        message = RCJSON["messages"][postCurr]["msg"].ToString(),
                                        create_at = Convert.ToInt64(ReplyUnixDateTime)
                                    });
                                    replyArray.Add(replyObject);
                                    replyCurr++;
                                    RCJSON["messages"][postCurr] = "";
                                    if (replyCurr == replyMax)
                                    {
                                        postRoot["post"].Add("replies", replyArray);
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    if (RCJSON["messages"][j]["reactions"] != null)
                    {
                        reactionsArray = new JArray();
                        foreach (var emoji in RCJSON["messages"][j]["reactions"])
                        {
                            if (emoji.ToString().Contains(":good:"))
                            {
                                ReactionsJson("thumbsup", emoji.Parent[":good:"]["usernames"], Convert.ToInt64(unixDateTime));
                            }
                            else if (emoji.ToString().Contains(":thumbup:"))
                            {
                                ReactionsJson("thumbsup", emoji.Parent[":thumbup:"]["usernames"], Convert.ToInt64(unixDateTime));

                            }
                            else if (emoji.ToString().Contains(":thumbsup:"))
                            {
                                ReactionsJson("thumbsup", emoji.Parent[":thumbsup:"]["usernames"], Convert.ToInt64(unixDateTime));

                            }
                            else if (emoji.ToString().Contains(":partying_face:"))
                            {
                                ReactionsJson("drooling_face", emoji.Parent[":partying_face:"]["usernames"], Convert.ToInt64(unixDateTime));

                            }
                            else if (emoji.ToString().Contains(":flag_nl:"))
                            {
                                ReactionsJson("netherlands", emoji.Parent[":flag_nl:"]["usernames"], Convert.ToInt64(unixDateTime));

                            }
                            else if (emoji.ToString().Contains(":biggrin:"))
                            {
                                ReactionsJson("laughing", emoji.Parent[":biggrin:"]["usernames"], Convert.ToInt64(unixDateTime));

                            }
                            else if (emoji.ToString().Contains(":bad:"))
                            {
                                ReactionsJson("nauseated_face", emoji.Parent[":bad:"]["usernames"], Convert.ToInt64(unixDateTime));

                            }
                            else if (emoji.ToString().Contains(":digit_one:"))
                            {
                                ReactionsJson("one", emoji.Parent[":digit_one:"]["usernames"], Convert.ToInt64(unixDateTime));

                            }
                            else if (emoji.ToString().Contains(":zany_face:"))
                            {
                                ReactionsJson("dizzy_face", emoji.Parent[":zany_face:"]["usernames"], Convert.ToInt64(unixDateTime));

                            }
                            else if (emoji.ToString().Contains(":bugurt:"))
                            {
                                ReactionsJson("smirk", emoji.Parent[":bugurt:"]["usernames"], Convert.ToInt64(unixDateTime));

                            }
                            else if (emoji.ToString().Contains(":lol:"))
                            {
                                ReactionsJson("laughing", emoji.Parent[":lol:"]["usernames"], Convert.ToInt64(unixDateTime));

                            }
                            else if (emoji.ToString().Contains(":tada:"))
                            {
                                ReactionsJson("tada", emoji.Parent[":tada:"]["usernames"], Convert.ToInt64(unixDateTime));

                            }
                            else if (emoji.ToString().Contains(":joy:"))
                            {
                                ReactionsJson("joy", emoji.Parent[":joy:"]["usernames"], Convert.ToInt64(unixDateTime));

                            }
                            else if (emoji.ToString().Contains(":girl_claping:"))
                            {
                                ReactionsJson("girl_claping", emoji.Parent[":girl_claping:"]["usernames"], Convert.ToInt64(unixDateTime));

                            }
                            else if (emoji.ToString().Contains(":clapping:"))
                            {
                                ReactionsJson("clapping", emoji.Parent[":clapping:"]["usernames"], Convert.ToInt64(unixDateTime));

                            }
                            else if (emoji.ToString().Contains(":christmas_tree:"))
                            {
                                ReactionsJson("christmas_tree", emoji.Parent[":christmas_tree:"]["usernames"], Convert.ToInt64(unixDateTime));


                            }
                            else if (emoji.ToString().Contains(":bee:"))
                            {
                                ReactionsJson("bee", emoji.Parent[":bee:"]["usernames"], Convert.ToInt64(unixDateTime));


                            }
                            else if (emoji.ToString().Contains(":heavy_plus_sign:"))
                            {
                                ReactionsJson("heavy_plus_sign", emoji.Parent[":heavy_plus_sign:"]["usernames"], Convert.ToInt64(unixDateTime));

                            }
                            else if (emoji.ToString().Contains(":gamer:"))
                            {
                                ReactionsJson("video_game", emoji.Parent[":gamer:"]["usernames"], Convert.ToInt64(unixDateTime));

                            }
                            else if (emoji.ToString().Contains(":ok_hand:"))
                            {
                                ReactionsJson("ok_hand", emoji.Parent[":ok_hand:"]["usernames"], Convert.ToInt64(unixDateTime));

                            }
                            else if (emoji.ToString().Contains(":+1:"))
                            {
                                ReactionsJson("+1", emoji.Parent[":+1:"]["usernames"], Convert.ToInt64(unixDateTime));

                            }
                            else if (emoji.ToString().Contains(":relaxed:"))
                            {
                                ReactionsJson("relaxed", emoji.Parent[":relaxed:"]["usernames"], Convert.ToInt64(unixDateTime));

                            }
                            else if (emoji.ToString().Contains(":yes:"))
                            {
                                ReactionsJson("thumbsup", emoji.Parent[":yes:"]["usernames"], Convert.ToInt64(unixDateTime));

                            }
                            else if (emoji.ToString().Contains(":dart:"))
                            {
                                ReactionsJson("dart", emoji.Parent[":dart:"]["usernames"], Convert.ToInt64(unixDateTime));
                            }
                            else if (emoji.ToString().Contains(":grin:"))
                            {
                                ReactionsJson("grin", emoji.Parent[":grin:"]["usernames"], Convert.ToInt64(unixDateTime));
                            }
                            else if (emoji.ToString().Contains(":thumbsdown:"))
                            {
                                ReactionsJson("thumbsdown", emoji.Parent[":thumbsdown:"]["usernames"], Convert.ToInt64(unixDateTime));
                            }
                            else if (emoji.ToString().Contains(":i_am_so_happy:"))
                            {
                                ReactionsJson("relaxed", emoji.Parent[":i_am_so_happy:"]["usernames"], Convert.ToInt64(unixDateTime));
                            }
                            else if (emoji.ToString().Contains(":girl_dance:"))
                            {
                                ReactionsJson("dancing_women", emoji.Parent[":girl_dance:"]["usernames"], Convert.ToInt64(unixDateTime));
                            }
                            else if (emoji.ToString().Contains(":raised_hands:"))
                            {
                                ReactionsJson("raised_hands", emoji.Parent[":raised_hands:"]["usernames"], Convert.ToInt64(unixDateTime));
                            }
                            else if (emoji.ToString().Contains(":flag_of_truce:"))
                            {
                                ReactionsJson("white_flag", emoji.Parent[":flag_of_truce:"]["usernames"], Convert.ToInt64(unixDateTime));
                            }
                            else if (emoji.ToString().Contains(":drinks:"))
                            {
                                ReactionsJson("tropical_drink", emoji.Parent[":drinks:"]["usernames"], Convert.ToInt64(unixDateTime));
                            }
                            else if (emoji.ToString().Contains(":sweat_smile:"))
                            {
                                ReactionsJson("sweat_smile", emoji.Parent[":sweat_smile:"]["usernames"], Convert.ToInt64(unixDateTime));
                            }
                            else if (emoji.ToString().Contains(":heart_eyes:"))
                            {
                                ReactionsJson("heart_eyes", emoji.Parent[":heart_eyes:"]["usernames"], Convert.ToInt64(unixDateTime));
                            }
                            else if (emoji.ToString().Contains(":open_mouth:"))
                            {
                                ReactionsJson("open_mouth", emoji.Parent[":open_mouth:"]["usernames"], Convert.ToInt64(unixDateTime));
                            }
                            else if (emoji.ToString().Contains(":bye:"))
                            {
                                ReactionsJson("waving", emoji.Parent[":bye:"]["usernames"], Convert.ToInt64(unixDateTime));
                            }
                            else if (emoji.ToString().Contains(":pray:"))
                            {
                                ReactionsJson("pray", emoji.Parent[":pray:"]["usernames"], Convert.ToInt64(unixDateTime));
                            }
                            else if (emoji.ToString().Contains(":grinning:"))
                            {
                                ReactionsJson("grinning", emoji.Parent[":grinning:"]["usernames"], Convert.ToInt64(unixDateTime));
                            }
                            else if (emoji.ToString().Contains(":thinking:"))
                            {
                                ReactionsJson("thinking", emoji.Parent[":thinking:"]["usernames"], Convert.ToInt64(unixDateTime));
                            }
                        }
                        if (reactionsArray.Count > 0)
                        {
                            postRoot["post"].Add("reactions", reactionsArray);

                        }
                    }
                    if (RCJSON["messages"][j]["attachments"] != null)
                    {
                        var attachmentsArray = new JArray();
                        var attachmentsPropsArray = new JArray();
                        foreach (var attachment in RCJSON["messages"][j]["attachments"])
                        {
                            var client = new WebClient();
                            client.Headers.Add(HttpRequestHeader.Cookie,
                                "rc_uid="+rcuid+";" +
                                "rc_token="+rctoken);
                            
                            if ((attachment["title_link"] != null) || (attachment["image_link"] != null))
                            {
                                
                                var link = rchome + attachment["title_link"].ToString();
                                var filename = attachment["title"].ToString();
                                if (attachment["image_type"] != null)
                                {
                                    if (attachment["image_type"].ToString().Contains("png"))
                                    {
                                        if (filename.Substring(filename.Length - 3, 3) != "png")
                                        {
                                            filename = filename + ".png";
                                        }
                                    }
                                    else if (attachment["image_type"].ToString().Contains("jpeg"))
                                    {
                                        if (filename.Substring(filename.Length - 4, 4) != "jpeg")
                                        {
                                            filename = filename + ".jpeg";
                                        }
                                    }
                                }

                                foreach (char c in Path.GetInvalidFileNameChars())
                                {
                                    filename = filename.Replace(c, '_').Replace(" ", "");
                                }

                                client.DownloadFile(link, outputdir+filename);
                                var attachmentsObject = JObject.FromObject(new
                                {
                                    path = importdir + filename
                                });
                                var attachmentsProp = JObject.FromObject(new
                                {
                                    text = attachment["title"]
                                });
                                attachmentsArray.Add(attachmentsObject);
                                attachmentsPropsArray.Add(attachmentsProp);
                            } 
                        }
                        if (attachmentsPropsArray.Count > 0)
                        {
                            postRoot["post"]["props"].Add("attachments", attachmentsPropsArray);
                        }
                        postRoot["post"].Add("attachments", attachmentsArray);
                    }
                    if (postRoot["type"] != null)
                    {
                        if (j > 2)
                        {
                            File.AppendAllText(importjsonl, postRoot.ToString(Formatting.None) + Environment.NewLine);
                        }
                        else
                        {
                            File.AppendAllText(importjsonl, postRoot.ToString(Formatting.None));
                        }
                        Console.Write("\rDoing Message {0} out of {1}", postMax - j + 1, postMax + 1);
                    }
                }
            }
            static void ReactionsJson(string emoji, JToken userList, long unixDateTime)
            {
                JObject reactionObject = null;
                foreach (var username in userList)
                {
                    reactionObject = JObject.FromObject(new
                    {
                        user = username,
                        emoji_name = emoji,
                        create_at = unixDateTime
                    });
                    reactionsArray.Add(reactionObject);
                }
            }


        }
    }
}
