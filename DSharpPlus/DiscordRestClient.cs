﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using DSharpPlus.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DSharpPlus
{
    internal sealed class DiscordRestClient
    {
        internal DiscordClient Discord { get; }
        internal RestClient Rest { get; }

        internal DiscordRestClient(DiscordClient client)
        {
            this.Discord = client;
            this.Rest = new RestClient(client);
        }

        #region Guild
        internal async Task<DiscordGuild> InternalCreateGuildAsync(string name)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Guilds;
            var headers = Utils.GetBaseHeaders();
            JObject payload = new JObject { { "name", name } };

            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.POST, headers, payload.ToString());
            WebResponse response = await this.Rest.HandleRequestAsync(request);

            DiscordGuild guild = JsonConvert.DeserializeObject<DiscordGuild>(response.Response);
            return guild;
        }


        internal async void InternalDeleteGuildAsync(ulong id)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Guilds + $"/{id}";
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.DELETE, headers);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
        }

        internal async Task<DiscordGuild> InternalModifyGuild(ulong guild_id, string name = "", string region = "", int verification_level = -1, int default_message_notifications = -1,
            ulong afk_channel_id = 0, int afk_timeout = -1, string icon = "", ulong owner_id = 0, string splash = "")
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Guilds + "/" + guild_id;
            var headers = Utils.GetBaseHeaders();
            JObject j = new JObject();
            if (name != "")
                j.Add("name", name);
            if (region != "")
                j.Add("region", region);
            if (verification_level != -1)
                j.Add("verification_level", verification_level);
            if (default_message_notifications != -1)
                j.Add("default_message_notifications", default_message_notifications);
            if (afk_channel_id != 0)
                j.Add("akf_channel_id", afk_channel_id);
            if (afk_timeout != -1)
                j.Add("akf_timeout", afk_timeout);
            if (icon != "")
                j.Add("icon", icon);
            if (owner_id != 0)
                j.Add("owner_id", owner_id);
            if (splash != "")
                j.Add("splash", splash);

            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.PATCH, headers, j.ToString());
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            return JsonConvert.DeserializeObject<DiscordGuild>(response.Response);
        }

        internal async Task<List<DiscordMember>> InternalGetGuildBans(ulong guild_id)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Guilds + "/" + guild_id + Endpoints.Bans;
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.GET, headers);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            JArray j = JArray.Parse(response.Response);
            List<DiscordMember> bans = new List<DiscordMember>();
            foreach (JObject obj in j)
            {
                bans.Add(obj.ToObject<DiscordMember>());
            }
            return bans;
        }

        internal async Task InternalCreateGuildBan(ulong guild_id, ulong user_id)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Guilds + "/" + guild_id + Endpoints.Bans + "/" + user_id;
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.PUT, headers);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
        }

        internal async Task InternalRemoveGuildBan(ulong guild_id, ulong user_id)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Guilds + "/" + guild_id + Endpoints.Bans + "/" + user_id;
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.DELETE, headers);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
        }

        internal async Task InternalLeaveGuild(ulong guild_id)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Users + "/@me" + Endpoints.Guilds + "/" + guild_id;
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.DELETE, headers);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
        }

        internal async Task<DiscordGuild> InternalCreateGuild(string name, string region, string icon, int verification_level, int default_message_notifications)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Guilds;
            var headers = Utils.GetBaseHeaders();
            JObject j = new JObject
            {
                { "name", name },
                { "region", region },
                { "icon", icon },
                { "verification_level", verification_level },
                { "default_message_notifications", default_message_notifications }
            };
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.POST, headers, j.ToString());
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            return JsonConvert.DeserializeObject<DiscordGuild>(response.Response);
        }

        internal async Task<DiscordMember> InternalAddGuildMember(ulong guild_id, ulong user_id, string access_token, string nick = "", List<DiscordRole> roles = null,
        bool muted = false, bool deafened = false)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Guilds + "/" + guild_id + Endpoints.Members + "/" + user_id;
            var headers = Utils.GetBaseHeaders();
            JObject j = new JObject
            {
                { "access_token", access_token }
            };
            if (nick != "")
                j.Add("nick", nick);
            if (roles != null)
            {
                JArray r = new JArray();
                foreach (DiscordRole role in roles)
                {
                    r.Add(JsonConvert.SerializeObject(role));
                }
                j.Add("roles", r);
            }
            if (muted)
                j.Add("mute", true);
            if (deafened)
                j.Add("deaf", true);
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.PUT, headers, j.ToString());
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            return JsonConvert.DeserializeObject<DiscordMember>(response.Response);
        }

        internal async Task<List<DiscordMember>> InternalListGuildMembers(ulong guild_id, int limit, int after)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Guilds + "/" + guild_id + Endpoints.Members + $"?limit={limit}&after={after}";
            var headers = Utils.GetBaseHeaders();
            JObject j = new JObject();

            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.GET, headers, j.ToString());
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            JArray ja = JArray.Parse(response.Response);
            List<DiscordMember> members = new List<DiscordMember>();
            foreach (JObject m in ja)
            {
                members.Add(m.ToObject<DiscordMember>());
            }
            return members;
        }

        internal async Task InternalAddGuildMemberRole(ulong guild_id, ulong user_id, ulong role_id)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Guilds + "/" + guild_id + Endpoints.Members + $"/{user_id}" + Endpoints.Roles + $"/{role_id}";
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.PUT, headers);
            await this.Rest.HandleRequestAsync(request);
        }

        internal async Task InternalRemoveGuildMemberRole(ulong guild_id, ulong user_id, ulong role_id)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Guilds + "/" + guild_id + Endpoints.Members + $"/{user_id}" + Endpoints.Roles + $"/{role_id}";
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.DELETE, headers);
            await this.Rest.HandleRequestAsync(request);
        }
        #endregion

        #region Channel

        internal async Task<DiscordChannel> InternalCreateGuildChannelAsync(ulong id, string name, ChannelType type)
        {
            if (name.Length > 200 || name.Length < 2)
                throw new Exception("Channel names can't be longer than 200 or shorter than 2 characters!");

            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Guilds + $"/{id}" + Endpoints.Channels;
            var headers = Utils.GetBaseHeaders();
            JObject payload = new JObject { { "name", name }, { "type", type.ToString() }, { "permission_overwrites", null } };

            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.POST, headers, payload.ToString());
            WebResponse response = await this.Rest.HandleRequestAsync(request);

            return JsonConvert.DeserializeObject<DiscordChannel>(response.Response);
        }

        internal async Task<DiscordChannel> InternalGetChannel(ulong id)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Channels + "/" + id;
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.GET, headers);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            return JsonConvert.DeserializeObject<DiscordChannel>(response.Response);
        }

        internal async Task InternalDeleteChannel(ulong id)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Channels + "/" + id;
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.DELETE, headers);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
        }

        internal async Task<DiscordMessage> InternalGetMessage(ulong channel_id, ulong message_id)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Channels + "/" + channel_id + Endpoints.Messages + "/" + message_id;
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.GET, headers);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            return JsonConvert.DeserializeObject<DiscordMessage>(response.Response);
        }

        internal async Task<DiscordMessage> InternalCreateMessage(ulong channel_id, string content, bool tts, DiscordEmbed embed = null)
        {
            if (content.Length > 2000)
                throw new Exception("Messages are limited to a total of 2000 characters!");
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Channels + "/" + channel_id + Endpoints.Messages;
            JObject j = new JObject
            {
                { "content", content },
                { "tts", tts }
            };
            if (embed != null)
            {
                JObject jembed = JObject.FromObject(embed);
                if (embed.Timestamp == new DateTime())
                {
                    jembed.Remove("timestamp");
                }
                else
                {
                    jembed["timestamp"] = embed.Timestamp.ToUniversalTime().ToString("s", CultureInfo.InvariantCulture);
                }
                j.Add("embed", jembed);
            }
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.POST, headers, j.ToString());
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            return JsonConvert.DeserializeObject<DiscordMessage>(response.Response);
        }

        internal async Task<DiscordMessage> InternalUploadFile(ulong channel_id, Stream file_data, string file_name, string content = "", bool tts = false)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Channels + "/" + channel_id + Endpoints.Messages;
            var headers = Utils.GetBaseHeaders();
            var values = new Dictionary<string, string>();
            if (content != "")
                values.Add("content", content);
            if (tts)
                values.Add("tts", tts.ToString());
            WebRequest request = WebRequest.CreateMultipartRequest(url, HttpRequestMethod.POST, headers, values, file_data, file_name);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            return JsonConvert.DeserializeObject<DiscordMessage>(response.Response);
        }

        internal async Task<List<DiscordChannel>> InternalGetGuildChannels(ulong guild_id)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Guilds + "/" + guild_id + Endpoints.Channels;
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.GET, headers);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            JArray j = JArray.Parse(response.Response);
            List<DiscordChannel> channels = new List<DiscordChannel>();
            foreach (JObject jj in j)
            {
                channels.Add(JsonConvert.DeserializeObject<DiscordChannel>(jj.ToString()));
            }
            return channels;
        }

        internal async Task<DiscordChannel> InternalCreateChannel(ulong guild_id, string name, ChannelType type, int bitrate = 0, int user_limit = 0)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Guilds + "/" + guild_id + Endpoints.Channels;
            var headers = Utils.GetBaseHeaders();
            JObject j = new JObject
            {
                { "name", name },
                { "type", type == ChannelType.Text ? "text" : "voice" }
            };
            if (type == ChannelType.Voice)
            {
                j.Add("bitrate", bitrate);
                j.Add("userlimit", user_limit);
            }
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.POST, headers, j.ToString());
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            return JsonConvert.DeserializeObject<DiscordChannel>(response.Response);
        }

        // TODO
        internal async Task InternalModifyGuildChannelPosition(ulong guild_id, ulong channel_id, int position)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Guilds + "/" + guild_id + Endpoints.Channels;
            var headers = Utils.GetBaseHeaders();
            JObject j = new JObject
            {
                { "id", channel_id },
                { "position", position }
            };
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.PATCH, headers, j.ToString());
            WebResponse response = await this.Rest.HandleRequestAsync(request);
        }

        internal async Task<List<DiscordMessage>> InternalGetChannelMessages(ulong channel_id, ulong around = 0, ulong before = 0, ulong after = 0, int limit = -1)
        {
            // ONLY ONE OUT OF around, before or after MAY BE USED.
            // THESE ARE MESSAGE ID's

            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Channels + "/" + channel_id + Endpoints.Messages;
            var headers = Utils.GetBaseHeaders();
            JObject j = new JObject();
            if (around != 0)
                j.Add("around", around);
            if (before != 0)
                j.Add("before", before);
            if (after != 0)
                j.Add("after", after);
            if (limit > -1)
                j.Add("limit", limit);

            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.GET, headers, j.ToString());
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            JArray ja = JArray.Parse(response.Response);
            List<DiscordMessage> messages = new List<DiscordMessage>();
            foreach (JObject jo in ja)
            {
                messages.Add(jo.ToObject<DiscordMessage>());
            }
            return messages;
        }

        internal async Task<DiscordMessage> InternalGetChannelMessage(ulong channel_id, ulong message_id)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Channels + "/" + channel_id + Endpoints.Messages + "/" + message_id;
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.GET, headers);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            return JsonConvert.DeserializeObject<DiscordMessage>(response.Response);
        }

        // hi c:

        internal async Task<DiscordMessage> InternalEditMessage(ulong channel_id, ulong message_id, string content = null, DiscordEmbed embed = null)
        {
            if (content != null && content.Length > 2000)
                throw new Exception("Messages are limited to a total of 2000 characters!");
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Channels + "/" + channel_id + Endpoints.Messages + "/" + message_id;
            var headers = Utils.GetBaseHeaders();
            JObject j = new JObject();
            if (content != null)
                j.Add("content", content);
            if (embed != null)
            {
                JObject jembed = JObject.FromObject(embed);
                if (embed.Timestamp == new DateTime())
                {
                    jembed.Remove("timestamp");
                }
                else
                {
                    jembed["timestamp"] = embed.Timestamp.ToUniversalTime().ToString("s", CultureInfo.InvariantCulture);
                }
                j.Add("embed", jembed);
            }
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.PATCH, headers, j.ToString());
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            return JsonConvert.DeserializeObject<DiscordMessage>(response.Response);
        }

        internal async Task InternalDeleteMessage(ulong channel_id, ulong message_id)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Channels + "/" + channel_id + Endpoints.Messages + "/" + message_id;
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.DELETE, headers);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
        }

        internal async Task InternalBulkDeleteMessages(ulong channel_id, List<ulong> message_ids)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Channels + "/" + channel_id + Endpoints.Messages + Endpoints.BulkDelete;
            var headers = Utils.GetBaseHeaders();
            JObject j = new JObject();
            JArray msgs = new JArray();
            foreach (ulong messageID in message_ids)
            {
                msgs.Add(messageID);
            }
            j.Add("messages", msgs);
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.POST, headers, j.ToString());
            WebResponse response = await this.Rest.HandleRequestAsync(request);
        }

        internal async Task<List<DiscordInvite>> InternalGetChannelInvites(ulong channel_id)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Channels + "/" + channel_id + Endpoints.Invites;
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.GET, headers);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            JArray ja = JArray.Parse(response.Response);
            List<DiscordInvite> invites = new List<DiscordInvite>();
            foreach (JObject jo in ja)
            {
                invites.Add(JsonConvert.DeserializeObject<DiscordInvite>(jo.ToString()));
            }
            return invites;
        }

        internal async Task<DiscordInvite> InternalCreateChannelInvite(ulong channel_id, int max_age = 86400, int max_uses = 0, bool temporary = false, bool unique = false)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Channels + "/" + channel_id + Endpoints.Invites;
            var headers = Utils.GetBaseHeaders();
            JObject j = new JObject
            {
                { "max_age", max_age },
                { "max_uses", max_uses },
                { "temporary", temporary },
                { "unique", unique }
            };
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.POST, headers, j.ToString());
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            return JsonConvert.DeserializeObject<DiscordInvite>(response.Response);
        }

        internal async Task InternalDeleteChannelPermission(ulong channel_id, ulong overwrite_id)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Channels + "/" + channel_id + Endpoints.Permissions + "/" + overwrite_id;
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.DELETE, headers);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
        }

        internal async Task InternalTriggerTypingIndicator(ulong channel_id)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Channels + "/" + channel_id + Endpoints.Typing;
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.POST, headers);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
        }

        internal async Task<List<DiscordMessage>> InternalGetPinnedMessages(ulong channel_id)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Channels + "/" + channel_id + Endpoints.Pins;
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.GET, headers);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            JArray j = JArray.Parse(response.Response);
            List<DiscordMessage> messages = new List<DiscordMessage>();
            foreach (JObject obj in j)
            {
                messages.Add(JsonConvert.DeserializeObject<DiscordMessage>(obj.ToString()));
            }
            return messages;
        }

        internal async Task InternalAddPinnedChannelMessage(ulong channel_id, ulong message_id)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Channels + "/" + channel_id + Endpoints.Pins + "/" + message_id;
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.PUT, headers);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
        }

        internal async Task InternalDeletePinnedChannelMessage(ulong channel_id, ulong message_id)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Channels + "/" + channel_id + Endpoints.Pins + "/" + message_id;
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.DELETE, headers);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
        }

        internal async Task InternalGroupDMAddRecipient(ulong channel_id, ulong user_id, string access_token)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Channels + "/" + channel_id + Endpoints.Recipients + "/" + user_id;
            var headers = Utils.GetBaseHeaders();
            JObject j = new JObject
            {
                { "access_token", access_token }
            };
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.PUT, headers, j.ToString());
            WebResponse response = await this.Rest.HandleRequestAsync(request);
        }

        internal async Task InternalGroupDMRemoveRecipient(ulong channel_id, ulong user_id)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Channels + "/" + channel_id + Endpoints.Recipients + "/" + user_id;
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.DELETE, headers);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
        }


        internal async Task InternalEditChannelPermissions(ulong channel_id, ulong overwrite_id, Permission allow, Permission deny, string type)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Channels + "/" + channel_id + Endpoints.Permissions + "/" + overwrite_id;
            var headers = Utils.GetBaseHeaders();
            JObject j = new JObject
            {
                { "allow", (ulong)allow },
                { "deny", (ulong)deny },
                { "type", type }
            };
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.PUT, headers, j.ToString());
            WebResponse response = await this.Rest.HandleRequestAsync(request);
        }

        internal async Task<DiscordDMChannel> InternalCreateDM(ulong recipient_id)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Users + "/@me" + Endpoints.Channels;
            var headers = Utils.GetBaseHeaders();
            JObject j = new JObject
            {
                { "recipient_id", recipient_id }
            };
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.POST, headers, j.ToString());
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            return JsonConvert.DeserializeObject<DiscordDMChannel>(response.Response);
        }

        internal async Task<DiscordDMChannel> InternalCreateGroupDM(List<string> access_tokens)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Users + "/@me" + Endpoints.Channels;
            var headers = Utils.GetBaseHeaders();
            JArray tokens = new JArray();
            foreach (string token in access_tokens)
            {
                tokens.Add(token);
            }
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.POST, headers, tokens.ToString());
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            return JsonConvert.DeserializeObject<DiscordDMChannel>(response.Response);
        }
        #endregion

        #region Member
        internal async Task<List<DiscordMember>> InternalGetGuildMembers(ulong guild_id, int member_count)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Guilds + "/" + guild_id + Endpoints.Members;
            var headers = Utils.GetBaseHeaders();
            List<DiscordMember> result = new List<DiscordMember>();
            int pages = (int)Math.Ceiling((double)member_count / 1000);
            ulong lastId = 0;

            for (int i = 0; i < pages; i++)
            {
                WebRequest request = WebRequest.CreateRequest(this.Discord, $"{url}?limit=1000&after={lastId}", HttpRequestMethod.GET, headers);
                WebResponse response = await this.Rest.HandleRequestAsync(request);

                List<DiscordMember> items = JsonConvert.DeserializeObject<List<DiscordMember>>(response.Response);
                result.AddRange(items);
                lastId = items[items.Count - 1].Id;
            }
            return result;
        }

        internal Task<DiscordUser> InternalGetUser(ulong user) =>
            InternalGetUser(user.ToString());

        internal async Task<DiscordUser> InternalGetUser(string user)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Users + $"/{user}";
            var headers = Utils.GetBaseHeaders();

            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.GET, headers);
            WebResponse response = await this.Rest.HandleRequestAsync(request);

            return JsonConvert.DeserializeObject<DiscordUser>(response.Response);
        }

        internal async Task<DiscordMember> InternalGetGuildMember(ulong guild_id, ulong member_id)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Guilds + "/" + guild_id + Endpoints.Members + "/" + member_id;
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.GET, headers);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            return JsonConvert.DeserializeObject<DiscordMember>(response.Response);
        }

        internal async Task InternalRemoveGuildMember(ulong guild_id, ulong user_id)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Guilds + "/" + guild_id + Endpoints.Members + "/" + user_id;
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.DELETE, headers);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
        }

        internal async Task<DiscordUser> InternalGetCurrentUser()
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Users + "/@me";
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.GET, headers);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            return JsonConvert.DeserializeObject<DiscordUser>(response.Response);
        }

        internal async Task<DiscordUser> InternalModifyCurrentUser(string username = "", string base64_avatar = "")
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Users + "/@me";
            var headers = Utils.GetBaseHeaders();
            JObject j = new JObject();
            if (username != "")
                j.Add("", username);
            if (base64_avatar != "")
                j.Add("avatar", base64_avatar);
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.PATCH, headers, j.ToString());
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            return JsonConvert.DeserializeObject<DiscordUser>(response.Response);
        }

        internal async Task<List<DiscordGuild>> InternalGetCurrentUserGuilds()
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Users + "/@me" + Endpoints.Guilds;
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.GET, headers);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            List<DiscordGuild> guilds = new List<DiscordGuild>();
            foreach (JObject j in JArray.Parse(response.Response))
            {
                guilds.Add(JsonConvert.DeserializeObject<DiscordGuild>(j.ToString()));
            }
            return guilds;
        }

        internal async Task InternalModifyGuildMember(ulong guild_id, ulong user_id, string nick = null,
            List<ulong> roles = null, bool muted = false, bool deafened = false, ulong voicechannel_id = 0)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Guilds + "/" + guild_id + Endpoints.Members + "/" + user_id;
            var headers = Utils.GetBaseHeaders();
            JObject j = new JObject();
            if (nick != null)
                j.Add("nick", nick);
            if (roles != null)
            {
                JArray r = new JArray();
                foreach (ulong role in roles)
                {
                    r.Add(role);
                }
                j.Add("roles", r);
            }
            if (muted)
                j.Add("mute", true);
            if (deafened)
                j.Add("deaf", true);
            if (voicechannel_id != 0)
                j.Add("channel_id", voicechannel_id);
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.PATCH, headers, j.ToString());
            await this.Rest.HandleRequestAsync(request);
        }

        #endregion

        #region Roles
        // TODO
        internal List<DiscordRole> InternalGetGuildRoles(ulong guild_id)
        {
            return new List<DiscordRole>();
        }

        // TODO
        internal List<DiscordRole> InternalModifyGuildRolePosition(ulong guild_id, ulong id, int position)
        {
            return new List<DiscordRole>();
        }

        internal async Task<DiscordGuild> InternalGetGuildAsync(ulong guild_id)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Guilds + "/" + guild_id;
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.GET, headers);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            DiscordGuild guild = JsonConvert.DeserializeObject<DiscordGuild>(response.Response);
            if (this.Discord._guilds.ContainsKey(guild_id))
            {
                this.Discord._guilds[guild_id] = guild;
            }
            else
            {
                this.Discord._guilds.Add(guild.Id, guild);
            }
            return guild;
        }

        internal async Task<DiscordGuild> InternalModifyGuild(string name = "", string region = "", string icon = "", int verification_level = -1,
            int default_message_notifications = -1, ulong afk_channel_id = 0, int afk_timeout = -1, ulong owner_id = 0, string splash = "")
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Guilds;
            var headers = Utils.GetBaseHeaders();
            JObject j = new JObject();
            if (name != "")
                j.Add("name", name);
            if (region != "")
                j.Add("region", region);
            if (icon != "")
                j.Add("icon", icon);
            if (verification_level > -1)
                j.Add("verification_level", verification_level);
            if (default_message_notifications > -1)
                j.Add("default_message_notifications", default_message_notifications);
            if (afk_channel_id > 0)
                j.Add("afk_channel_id", afk_channel_id);
            if (afk_timeout > -1)
                j.Add("afk_timeout", afk_timeout);
            if (owner_id > 0)
                j.Add("owner_id", owner_id);
            if (splash != "")
                j.Add("splash", splash);

            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.PATCH, headers, j.ToString());
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            return JsonConvert.DeserializeObject<DiscordGuild>(response.Response);
        }

        internal async Task<DiscordGuild> InternalDeleteGuild(ulong guild_id)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Guilds + "/" + guild_id;
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.DELETE, headers);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            return JsonConvert.DeserializeObject<DiscordGuild>(response.Response);
        }

        internal async Task<DiscordRole> InternalModifyGuildRole(ulong guild_id, ulong role_id, string name, Permission permissions, int position, int color, bool separate, bool mentionable)
        {
            string url = $"{Utils.GetApiBaseUri(this.Discord)}{Endpoints.Guilds}/{guild_id}{Endpoints.Roles}/{role_id}";
            var headers = Utils.GetBaseHeaders();
            JObject j = new JObject
            {
                { "name", name },
                { "permissions", (ulong)permissions },
                { "position", position },
                { "color", color },
                { "hoist", separate },
                { "mentionable", mentionable }
            };
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.PATCH, headers, j.ToString());
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            return JsonConvert.DeserializeObject<DiscordRole>(response.Response);
        }

        internal async Task<DiscordRole> InternalDeleteRole(ulong guild_id, ulong role_id)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Guilds + "/" + guild_id + Endpoints.Roles + "/" + role_id;
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.DELETE, headers);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            return JsonConvert.DeserializeObject<DiscordRole>(response.Response);
        }

        internal async Task<DiscordRole> InternalCreateGuildRole(ulong guild_id)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Guilds + "/" + guild_id + Endpoints.Roles;
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.POST, headers);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            return JsonConvert.DeserializeObject<DiscordRole>(response.Response);
        }

        #endregion

        #region Prune
        // TODO
        internal async Task<int> InternalGetGuildPruneCount(ulong guild_id, int days)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Guilds + "/" + guild_id + Endpoints.Prune;
            var headers = Utils.GetBaseHeaders();
            JObject payload = new JObject
            {
                { "days", days }
            };
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.GET, headers, payload.ToString());
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            JObject j = JObject.Parse(response.Response);
            return int.Parse(j["pruned"].ToString());
        }

        // TODO
        internal async Task<int> InternalBeginGuildPrune(ulong guild_id, int days)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Guilds + "/" + guild_id + Endpoints.Prune;
            var headers = Utils.GetBaseHeaders();
            JObject payload = new JObject
            {
                { "days", days }
            };
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.POST, headers, payload.ToString());
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            JObject j = JObject.Parse(response.Response);
            return int.Parse(j["pruned"].ToString());
        }
        #endregion

        #region GuildVarious

        internal async Task<List<DiscordIntegration>> InternalGetGuildIntegrations(ulong guild_id)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Guilds + "/" + guild_id + Endpoints.Integrations;
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.GET, headers);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            JArray j = JArray.Parse(response.Response);
            List<DiscordIntegration> integrations = new List<DiscordIntegration>();
            foreach (JObject obj in j)
            {
                integrations.Add(JsonConvert.DeserializeObject<DiscordIntegration>(obj.ToString()));
            }
            return integrations;
        }

        internal async Task<DiscordIntegration> InternalCreateGuildIntegration(ulong guild_id, string type, ulong id)
        {
            // Attach from user
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Guilds + "/" + guild_id + Endpoints.Integrations;
            var headers = Utils.GetBaseHeaders();
            JObject j = new JObject
            {
                { "type", type },
                { "id", id }
            };
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.POST, headers, j.ToString());
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            return JsonConvert.DeserializeObject<DiscordIntegration>(response.Response);
        }

        internal async Task<DiscordIntegration> InternalModifyGuildIntegration(ulong guild_id, ulong integration_id, int expire_behaviour,
            int expire_grace_period, bool enable_emoticons)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Guilds + "/" + guild_id + Endpoints.Integrations + "/" + integration_id;
            JObject j = new JObject
            {
                { "expire_behaviour", expire_behaviour },
                { "expire_grace_period", expire_grace_period },
                { "enable_emoticons", enable_emoticons }
            };
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.PATCH, headers, j.ToString());
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            return JsonConvert.DeserializeObject<DiscordIntegration>(response.Response);
        }

        internal async Task InternalDeleteGuildIntegration(ulong guild_id, DiscordIntegration integration)
        {
            ulong IntegrationID = integration.Id;
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Guilds + "/" + guild_id + Endpoints.Integrations + "/" + IntegrationID;
            var headers = Utils.GetBaseHeaders();
            JObject j = JObject.FromObject(integration);
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.DELETE, headers, j.ToString());
            WebResponse response = await this.Rest.HandleRequestAsync(request);
        }

        internal async Task InternalSyncGuildIntegration(ulong guild_id, ulong integration_id)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Guilds + "/" + guild_id + Endpoints.Integrations + "/" + integration_id + Endpoints.Sync;
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.POST, headers);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
        }

        internal async Task<DiscordGuildEmbed> InternalGetGuildEmbed(ulong guild_id)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Guilds + "/" + guild_id + Endpoints.Embed;
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.GET, headers);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            return JsonConvert.DeserializeObject<DiscordGuildEmbed>(response.Response);
        }

        internal async Task<DiscordGuildEmbed> InternalModifyGuildEmbed(ulong guild_id, DiscordGuildEmbed embed)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Guilds + "/" + guild_id + Endpoints.Embed;
            var headers = Utils.GetBaseHeaders();
            JObject j = JObject.FromObject(embed);
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.PATCH, headers, j.ToString());
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            return JsonConvert.DeserializeObject<DiscordGuildEmbed>(response.Response);
        }

        internal async Task<List<DiscordVoiceRegion>> InternalGetGuildVoiceRegions(ulong guild_id)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Guilds + "/" + guild_id + Endpoints.Regions;
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.GET, headers);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            JArray j = JArray.Parse(response.Response);
            List<DiscordVoiceRegion> regions = new List<DiscordVoiceRegion>();
            foreach (JObject obj in j)
            {
                regions.Add(JsonConvert.DeserializeObject<DiscordVoiceRegion>(obj.ToString()));
            }
            return regions;
        }

        internal async Task<List<DiscordInvite>> InternalGetGuildInvites(ulong guild_id)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Guilds + "/" + guild_id + Endpoints.Invites;
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.GET, headers);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            JArray j = JArray.Parse(response.Response);
            List<DiscordInvite> invites = new List<DiscordInvite>();
            foreach (JObject obj in j)
            {
                invites.Add(JsonConvert.DeserializeObject<DiscordInvite>(obj.ToString()));
            }
            return invites;
        }

        #endregion

        #region Invite
        internal async Task<DiscordInvite> InternalGetInvite(string invite_code)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Invites + "/" + invite_code;
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.GET, headers);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            return JsonConvert.DeserializeObject<DiscordInvite>(response.Response);
        }

        internal async Task<DiscordInvite> InternalDeleteInvite(string invite_code)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Invites + "/" + invite_code;
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.DELETE, headers);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            return JsonConvert.DeserializeObject<DiscordInvite>(response.Response);
        }

        internal async Task<DiscordInvite> InternalAcceptInvite(string invite_code)
        {
            // USER ONLY
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Invites + "/" + invite_code;
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.POST, headers);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            return JsonConvert.DeserializeObject<DiscordInvite>(response.Response);
        }
        #endregion

        #region Connections
        internal async Task<List<DiscordConnection>> InternalGetUsersConnections()
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Users + "/@me" + Endpoints.Connections;
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.GET, headers);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            List<DiscordConnection> connections = new List<DiscordConnection>();
            foreach (JObject j in JArray.Parse(response.Response))
            {
                connections.Add(JsonConvert.DeserializeObject<DiscordConnection>(j.ToString()));
            }
            return connections;
        }
        #endregion

        #region Voice
        internal async Task<List<DiscordVoiceRegion>> InternalListVoiceRegions()
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Voice + Endpoints.Regions;
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.GET, headers);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            List<DiscordVoiceRegion> regions = new List<DiscordVoiceRegion>();
            JArray j = JArray.Parse(response.Response);
            foreach (JObject obj in j)
            {
                regions.Add(JsonConvert.DeserializeObject<DiscordVoiceRegion>(obj.ToString()));
            }
            return regions;
        }
        #endregion

        #region Webhooks
        internal async Task<DiscordWebhook> InternalCreateWebhook(ulong channel_id, string name, string base64_avatar)
        {
            if (name.Length > 200 || name.Length < 2)
                throw new Exception("Webhook name has to be between 2 and 200 characters!");

            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Channels + "/" + channel_id + Endpoints.Webhooks;
            var headers = Utils.GetBaseHeaders();
            JObject j = new JObject
            {
                { "name", name },
                { "avatar", base64_avatar }
            };
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.POST, headers, j.ToString());
            WebResponse response = await this.Rest.HandleRequestAsync(request);

            return JsonConvert.DeserializeObject<DiscordWebhook>(response.Response);
        }

        internal async Task<List<DiscordWebhook>> InternalGetChannelWebhooks(ulong channel_id)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Channels + "/" + channel_id + Endpoints.Webhooks;
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.GET, headers);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            List<DiscordWebhook> webhooks = new List<DiscordWebhook>();
            foreach (JObject j in JArray.Parse(response.Response))
            {
                webhooks.Add(JsonConvert.DeserializeObject<DiscordWebhook>(j.ToString()));
            }
            return webhooks;
        }

        internal async Task<List<DiscordWebhook>> InternalGetGuildWebhooks(ulong guild_id)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Guilds + "/" + guild_id + Endpoints.Webhooks;
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.GET, headers);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            List<DiscordWebhook> webhooks = new List<DiscordWebhook>();
            foreach (JObject j in JArray.Parse(response.Response))
            {
                webhooks.Add(JsonConvert.DeserializeObject<DiscordWebhook>(j.ToString()));
            }
            return webhooks;
        }

        internal async Task<DiscordWebhook> InternalGetWebhook(ulong webhook_id)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Webhooks + "/" + webhook_id;
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.GET, headers);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            return JsonConvert.DeserializeObject<DiscordWebhook>(response.Response);
        }

        // Auth header not required
        internal async Task<DiscordWebhook> InternalGetWebhookWithToken(ulong webhook_id, string webhook_token)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Webhooks + "/" + webhook_id + "/" + webhook_token;
            WebRequest request = WebRequest.CreateRequest(this.Discord, url);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            DiscordWebhook wh = JsonConvert.DeserializeObject<DiscordWebhook>(response.Response);
            wh.Token = webhook_token;
            wh.Id = webhook_id;
            return wh;
        }

        internal async Task<DiscordWebhook> InternalModifyWebhook(ulong webhook_id, string name, string base64_avatar)
        {
            if (name.Length > 200 || name.Length < 2)
                throw new Exception("Webhook name has to be between 2 and 200 characters!");
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Webhooks + "/" + webhook_id;
            var headers = Utils.GetBaseHeaders();
            JObject j = new JObject
            {
                { "name", name },
                { "avatar", base64_avatar }
            };
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.PATCH, headers, j.ToString());
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            return JsonConvert.DeserializeObject<DiscordWebhook>(response.Response);
        }

        internal async Task<DiscordWebhook> InternalModifyWebhook(ulong webhook_id, string name, string base64_avatar, string webhook_token)
        {
            if (name.Length > 200 || name.Length < 2)
                throw new Exception("Webhook name has to be between 2 and 200 characters!");
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Webhooks + "/" + webhook_id + "/" + webhook_token;
            JObject j = new JObject
            {
                { "name", name },
                { "avatar", base64_avatar }
            };
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.PATCH, payload: j.ToString());
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            return JsonConvert.DeserializeObject<DiscordWebhook>(response.Response);
        }

        internal async Task InternalDeleteWebhook(ulong webhook_id)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Webhooks + "/" + webhook_id;
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.DELETE, headers);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
        }

        internal async Task InternalDeleteWebhook(ulong webhook_id, string webhook_token)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Webhooks + "/" + webhook_id + "/" + webhook_token;
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.DELETE);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
        }

        internal async Task InternalExecuteWebhook(ulong webhook_id, string webhook_token, string content = "", string username = "", string avatar_url = "",
            bool tts = false, List<DiscordEmbed> embeds = null)
        {
            if (content.Length > 2000)
                throw new Exception("Messages are limited to a total of 2000 characters!");

            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Webhooks + "/" + webhook_id + "/" + webhook_token;
            JObject req = new JObject();
            if (content != "")
                req.Add("content", content);
            if (username != "")
                req.Add("username", username);
            if (avatar_url != "")
                req.Add("avatar_url", avatar_url);
            if (tts)
                req.Add("tts", tts);
            if (embeds != null)
            {
                JArray arr = new JArray();
                foreach (DiscordEmbed e in embeds)
                {
                    arr.Add(JsonConvert.SerializeObject(e));
                }
                req.Add(arr);
            }
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.POST, payload: req.ToString());
            WebResponse response = await this.Rest.HandleRequestAsync(request);
        }

        internal async Task InternalExecuteWebhookSlack(ulong webhook_id, string webhook_token, string json_payload)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Webhooks + "/" + webhook_id + "/" + webhook_token + Endpoints.Slack;
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.POST, payload: json_payload);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
        }

        internal async Task InternalExecuteWebhookGithub(ulong webhook_id, string webhook_token, string json_payload)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Webhooks + "/" + webhook_id + "/" + webhook_token + Endpoints.Github;
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.POST, payload: json_payload);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
        }

        #endregion

        #region Reactions
        internal async Task InternalCreateReaction(ulong channel_id, ulong message_id, string emoji)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Channels + "/" + channel_id + Endpoints.Messages + "/" + message_id + Endpoints.Reactions + "/" + emoji + "/@me";
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.PUT, headers);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
        }

        internal async Task InternalDeleteOwnReaction(ulong channel_id, ulong message_id, string emoji)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Channels + "/" + channel_id + Endpoints.Messages + "/" + message_id + Endpoints.Reactions + "/" + emoji + "/@me";
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.DELETE, headers);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
        }

        internal async Task InternalDeleteUserReaction(ulong channel_id, ulong message_id, ulong user_id, string emoji)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Channels + "/" + channel_id + Endpoints.Messages + "/" + message_id + Endpoints.Reactions + "/" + emoji + "/" + user_id;
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.DELETE, headers);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
        }

        internal async Task<List<DiscordUser>> InternalGetReactions(ulong channel_id, ulong message_id, string emoji)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Channels + "/" + channel_id + Endpoints.Messages + "/" + message_id + Endpoints.Reactions + "/" + emoji;
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.GET, headers);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            List<DiscordUser> reacters = new List<DiscordUser>();
            foreach (JObject obj in JArray.Parse(response.Response))
            {
                reacters.Add(obj.ToObject<DiscordUser>());
            }
            return reacters;
        }

        internal async Task InternalDeleteAllReactions(ulong channel_id, ulong message_id)
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Channels + "/" + channel_id + Endpoints.Messages + "/" + message_id + Endpoints.Reactions;
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.DELETE, headers);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
        }
        #endregion


        #region Misc
        internal async Task<DiscordApplication> InternalGetApplicationInfo(string id = "@me")
        {
            string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.OAuth2 + Endpoints.Applications + "/" + id;
            var headers = Utils.GetBaseHeaders();
            WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.GET, headers);
            WebResponse response = await this.Rest.HandleRequestAsync(request);
            return JObject.Parse(response.Response).ToObject<DiscordApplication>();
        }

        internal async Task InternalSetAvatarAsync(Stream image)
        {
            using (var ms = new MemoryStream())
            {
                await image.CopyToAsync(ms);
                var b64 = Convert.ToBase64String(ms.ToArray());

                string url = Utils.GetApiBaseUri(this.Discord) + Endpoints.Users + "/@me";
                var headers = Utils.GetBaseHeaders();
                JObject jo = new JObject
                {
                    { "avatar", $"data:image/jpeg;base64,{b64}" }
                };
                WebRequest request = WebRequest.CreateRequest(this.Discord, url, HttpRequestMethod.PATCH, headers, jo.ToString());
                WebResponse response = await this.Rest.HandleRequestAsync(request);
            }
        }
        #endregion
    }
}
