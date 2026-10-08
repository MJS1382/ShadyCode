using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalBlog.Data;

public static class BlogEndpoints
{
    public static RouteGroupBuilder MapBlogEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/like/{postId}",
            async (
                [FromServices] ApplicationDbContext DbContext,
                [FromServices] IHttpContextAccessor HttpContextAccessor,
                [FromRoute] string postId
            ) => {
                if(!Guid.TryParse(postId, out var postGuid))
                {
                    return Results.BadRequest("Invalid post ID.");
                }

                var loggedInUser = HttpContextAccessor.HttpContext?.User;
                if (loggedInUser?.Identity?.IsAuthenticated != true)
                {
                    return Results.Unauthorized();
                }

                var username = loggedInUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                if (username is null)
                {
                    return Results.Unauthorized();
                }

                var user = await DbContext.Users.FirstOrDefaultAsync(u => u.UserName == username);
                if (user is null)
                {
                    return Results.Unauthorized();
                }

                var post = await DbContext.Posts
                    .Include(p => p.LikedByUsers)
                    .FirstOrDefaultAsync(p => p.Id == postGuid);

                if (post == null)
                {
                    return Results.NotFound("Post not found.");
                }

                var liked = !post.LikedByUsers.Any(u => u.Id == user.Id);
                if (liked)
                {
                    post.LikedByUsers.Add(user);
                }
                else
                {
                    post.LikedByUsers.Remove(user);
                }

                post.Likes = post.LikedByUsers.Count;
                await DbContext.SaveChangesAsync();

                return Results.Ok(new { liked, likeCount = post.Likes });
            });

        return group;
    }
}