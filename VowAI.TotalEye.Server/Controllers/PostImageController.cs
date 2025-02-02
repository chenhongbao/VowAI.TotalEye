﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VowAI.TotalEye.Server.Models;
using VowAI.TotalEye.ServerShared.Models;
using VowAI.TotalEye.Tools;

namespace VowAI.TotalEye.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostImageController : ControllerBase
    {
        private readonly IDbContextFactory<ServerDbContext> _dbFactory;

        public PostImageController(IDbContextFactory<ServerDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        [HttpPost]
        public async Task<ActionResult<ClientControlPolicySet>> Post([FromForm] string token, IFormFile payload)
        {
            try
            {
                using (ServerDbContext context = _dbFactory.CreateDbContext())
                {
                    UserInfoSession? session = await context.Sessions
                        .Include(s => s.Request)
                        .ThenInclude(r => r.User)
                        .ThenInclude(u => u.Policies)
                        .FirstOrDefaultAsync<UserInfoSession>(session => session.Token == token);

                    if (session == null || session == default)
                    {
                        throw new ArgumentException($"Unknown token '{token}'.");
                    }
                    else
                    {
                        SessionScreenshot screen = new SessionScreenshot
                        {
                            Image = BuildImageItem(payload),
                            Session = session,
                        };

                        context.Add(screen);
                        await context.SaveChangesAsync();

                        SessionScreenshot? storedScreenshot = await context.Screenshots
                            .Include(cmd => cmd.Session)
                            .FirstOrDefaultAsync<SessionScreenshot>(cmd => cmd.Session == null ? false : cmd.Session.SessionId == session.SessionId);

                        session.Screenshot = storedScreenshot;
                        await context.SaveChangesAsync();

                        if (session.Request?.User != null)
                        {
                            return Redirect(GetControlPolicyController.GetControllerName() + "?userId=" + session.Request.User.UserId + "&pin=" + session.Request.User.Pin);
                        }
                        else
                        {
                            throw new InvalidDataException("Missing user property in session.");
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                exception.WriteString<PostImageController>();
                return BadRequest(exception);
            }
        }

        private ImageItem BuildImageItem(IFormFile payload)
        {
            ImageItem imageItem = new ImageItem
            {
                FileName = payload.FileName,
                Data = new byte[payload.Length],
            };

            using (Stream buffer = new MemoryStream(imageItem.Data))
            {
                payload.CopyToAsync(buffer);
            }

            return imageItem;
        }
    }
}
