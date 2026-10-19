using System.Security.Claims;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using PersonalBlog.Data;
using PersonalBlog.Models;

public static class BlogEndpoints
{
    public static RouteGroupBuilder MapBlogEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/like/{postId:guid}",
            async (
                [FromServices] ApplicationDbContext DbContext,
                [FromServices] IHttpContextAccessor HttpContextAccessor,
                [FromRoute] Guid postId
            ) => {
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
                    .FirstOrDefaultAsync(p => p.Id == postId);

                if (post == null)
                {
                    return Results.NotFound("Post not found.");
                }

                var liked = !post.LikedByUsers.Any(u => u.Id == user.Id);
                if (liked)
                {
                    post.LikedByUsers.Add(user);
                    post.Likes++;
                }
                else
                {
                    post.LikedByUsers.Remove(user);
                    post.Likes = Math.Max(0, post.Likes - 1);
                }

                post.Likes = post.LikedByUsers.Count;
                await DbContext.SaveChangesAsync();

                return Results.Ok(new { liked, likeCount = post.Likes });
            }).RequireAuthorization();

            group.MapGet("/likes/{postId:guid}", async (Guid postId, ApplicationDbContext db) =>
            {
                var Post = await db.Posts.FirstOrDefaultAsync(p => p.Id == postId);
                var likeCount = Post is null ? 0 : Post.Likes;

                return Results.Ok(new { likeCount });
            });

            group.MapGet("/comments/{postId:guid}",
                async (
                    [FromServices] ApplicationDbContext DbContext,
                    [FromServices] IHttpContextAccessor HttpContextAccessor,
                    [FromRoute] Guid postId,
                    [FromQuery] int page
                ) =>
                {
                    if (page < 0) return Results.BadRequest();

                    // AI GENERATED CODE BEGINS
                    Guid? currentUserId = null;
                    var username = HttpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);
                    if (!string.IsNullOrEmpty(username))
                    {
                        currentUserId = await DbContext.Users
                            .Where(u => u.UserName == username)
                            .Select(u => (Guid?)u.Id)
                            .FirstOrDefaultAsync();
                    }
                    // AI GENERATED CODE ENDS

                    var comments = await DbContext.Comments
                        .AsNoTracking()
                        .Where(c => c.PostId == postId)
                        .OrderByDescending(c => c.Date)
                        .ThenBy(c => c.Id)
                        .Skip(page * 10)
                        .Take(10)
                        .Select(c => new
                        {
                            c.Id,
                            c.Text,
                            c.Date,
                            c.Likes,
                            // AI GENERATED CODE BEGINS
                            IsLikedByUser = currentUserId.HasValue &&
                                c.LikedByUsers.Any(u => u.Id == currentUserId.Value),
                            // AI GENERATED CODE ENDS
                            User = c.User == null
                                ? null
                                : new { c.User.UserName }
                        })
                        .ToListAsync();

                    return Results.Ok(comments);
                });


            group.MapPost("/comment/{postId:guid}", async (
                [FromServices] ApplicationDbContext db,
                [FromServices] IHttpContextAccessor httpContext,
                [FromRoute] Guid postId,
                [FromBody] CommentInputModel input
            ) =>
            {
                var username = httpContext.HttpContext?.User.FindFirstValue(ClaimTypes.Name);
                if (username is null)
                {
                    return Results.Unauthorized();
                }

                var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserName == username);
                if (user is null)
                {
                    return Results.Unauthorized();
                }

                var post = await db.Posts.AsNoTracking().FirstOrDefaultAsync();   
                if (post is null)
                {
                    return Results.NotFound("Post Not Fount");
                }

                var newComment = new Comment
                {
                    PostId = postId,
                    ApplicationUserId = user.Id,
                    Text = input.Content ?? string.Empty,
                    Date = DateTime.UtcNow,
                    Likes = 0
                };
                
                db.Comments.Add(newComment);
                await db.SaveChangesAsync();

                return Results.Ok(new
                {
                    newComment.Id,
                    newComment.Text,
                    newComment.Date,
                    newComment.Likes,
                    User = user.UserName
                });
            }).RequireAuthorization();

            group.MapPost("/comments/like/{commentId:guid}",
            async (
                [FromServices] ApplicationDbContext DbContext,
                [FromServices] IHttpContextAccessor httpContext,
                [FromRoute] Guid commentId
            ) =>
            {
                var loggedInUsername = httpContext.HttpContext?.User.FindFirstValue(ClaimTypes.Name);
                if (string.IsNullOrEmpty(loggedInUsername))
                {
                    return Results.Unauthorized();
                }

                var user = await DbContext.Users.FirstOrDefaultAsync(u => u.UserName == loggedInUsername);
                if (user is null)
                {
                    return Results.Unauthorized();
                }

                var comment = await DbContext.Comments
                    .Include(c => c.LikedByUsers)
                    .FirstOrDefaultAsync(c => c.Id == commentId);

                if (comment is null)
                {
                    return Results.NotFound("Comment not found.");
                }

                var isLikedByUser = !comment.LikedByUsers.Any(u => u.Id == user.Id);
                if (isLikedByUser)
                {
                    comment.LikedByUsers.Add(user);
                }
                else
                {
                    comment.LikedByUsers.Remove(user);
                }

                comment.Likes = comment.LikedByUsers.Count;
                await DbContext.SaveChangesAsync();
                return Results.Ok(new { isLikedByUser, likeCount = comment.Likes });
            }).RequireAuthorization();

            group.MapGet("/comments/likes/{commentId:guid}",
            async (
                [FromServices] ApplicationDbContext db,
                [FromRoute] Guid commentId
            ) =>
            {
                var comment = await db.Comments.FirstOrDefaultAsync(c => c.Id == commentId);
                var likeCount = comment is null ? 0 : comment.Likes;

                return Results.Ok(new { likeCount });
            });

        return group;
    }
}