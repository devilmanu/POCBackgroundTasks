﻿using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobManager.hubs
{
    public class JobsHub : Hub
    {
        //public JobsHub()

        //public async Task Subscribe(int snippetId)
        //{
        //    await Groups.AddToGroupAsync(Context.ConnectionId, GetGroup(snippetId));

        //    // When a user subscribes a snippet that was already
        //    // highlighted, we need to send it immediately, because
        //    // otherwise she will listen for it infinitely.
        //    using (var db = new HighlighterDbContext())
        //    {
        //        var snippet = await db.CodeSnippets
        //            .Where(x => x.Id == snippetId && x.HighlightedCode != null)
        //            .SingleOrDefaultAsync();

        //        if (snippet != null)
        //        {
        //            Clients.Client(Context.ConnectionId)
        //                .highlight(snippet.Id, snippet.HighlightedCode);
        //        }
        //    }
        //}

        //public static string GetGroup(int snippetId)
        //{
        //    return "snippet:" + snippetId;
        //}
    }
}
