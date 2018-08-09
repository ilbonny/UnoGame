using System;

namespace UnoGame.Core.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string ConnectionHubId { get; set; }
        public string UserName { get; set; }
    }
}