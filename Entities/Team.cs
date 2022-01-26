using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace TeamPool.Entities
{
    public class Team
    {
        public string _teamId;
        public Color _teamColor;
        public  List<string> _membersIds; // ID kuleczek należących do team'u
        public Team(Color color)
        {
            _teamColor = color;
            _teamId = Guid.NewGuid().ToString();
            _membersIds = new List<string>();
        }

        public Color Color()
        {
            return _teamColor;
        }
        
        public void addMember(string newMemberId)
        {
            _membersIds.Add(newMemberId);
        }

        public bool isInTeam(string id)
        {
            return _membersIds.Contains(id);
        }
    }
}