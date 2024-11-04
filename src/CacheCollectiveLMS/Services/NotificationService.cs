using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;
using System.Security.Claims;

namespace RazorPagesMovie.Services
{
    public class NotificationService
    {
        private readonly RazorPagesMovieContext _context;
        public NotificationService(RazorPagesMovieContext context)
        {
            _context = context;
        }

        public async Task AddNotificationAsync(int userId, int courseId, int assignmentId, string description)
        {
            var notification = new Notification
            {
                UserId = userId,
                CourseId = courseId,
                AssignmentId = assignmentId,
                DateCreated = DateTime.UtcNow,
                Description = description
            };

            _context.Notification.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task MarkNotificationAsReadAsync(int notificationId)
        {
            var notification = await _context.Notification.Where(n => n.Id == notificationId).FirstOrDefaultAsync();
            if (notification != null)
            {
                notification.isRead = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Assignment?> GetAssignmentByIdAsync(int assignmentId)
        {
            return await _context.Assignment.Where(a => a.Id == assignmentId).FirstOrDefaultAsync();
        }

        public async Task<Course?> GetCourseByIdAsync(int courseId)
        {
            return await _context.Course.Where(c => c.CourseId == courseId).FirstOrDefaultAsync();
        }

        public async Task<List<Notification>> GetNotificationsAsync(int userId)
        {
            return await _context.Notification.Where(n => n.UserId == userId && !n.isRead).OrderByDescending(n => n.DateCreated).ToListAsync();
        }
    }
}
