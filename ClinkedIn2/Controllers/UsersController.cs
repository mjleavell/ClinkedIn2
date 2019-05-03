using System;
using System.Collections.Generic;
using System.Linq;
using ClinkedIn2.Data;
using ClinkedIn2.Models;
using ClinkedIn2.Validators;
using Microsoft.AspNetCore.Mvc;

namespace ClinkedIn2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        readonly UserRepository _userRepository;
        readonly CreateUserRequestValidator _validator;

        public UsersController()
        {
            _validator = new CreateUserRequestValidator();
            _userRepository = new UserRepository();
        }

        // -------------------------------- Users --------------------------------

        // Get All Users
        [HttpGet]
        public ActionResult GetUsers()
        {
            return Ok(_userRepository.GetAllUsers());
        }

        // Get Single User
        [HttpGet("{id}")]
        public ActionResult GetSingleUser(int id)
        {
            return Ok(_userRepository.GetSingleUser(id));
        }

        // Add User
        [HttpPost]
        public ActionResult AddUser(CreateUserRequest createRequest)
        {
            if (!_validator.Validate(createRequest))
            {
                return BadRequest(new { error = "users must have a username and password" });
            }
            var newUser = _userRepository.AddUser(createRequest.Username, createRequest.Password, createRequest.ReleaseDate, createRequest.Age);

            return Created($"api/users/{newUser.Id}", newUser);
        }

        // Delete User
        [HttpDelete("{id}")]
        public ActionResult DeleteUser(int Id)
        {
            _userRepository.DeleteUser(Id);
            return Ok();
        }

        // Get Update User IsPrisoner
        [HttpPut("{id}")]
        public ActionResult UpdateIsPrisoner(int id)
        {
            var newIsPrisoner = _userRepository.UpdateIsPrisoner(id);
            return Ok(newIsPrisoner);
        }

        // -------------------------------- Friends --------------------------------

        // Add Friend to User
        [HttpPut("{userId}/addFriend/{friendId}")]
        public ActionResult AddFriend(int userId, int friendId)
        {
            var users = _userRepository.GetAllUsers();
            var user = users.FirstOrDefault(u => u.Id == userId);
            var friendToAdd = users.FirstOrDefault(f => f.Id == friendId);

            if (!user.Friends.Contains(friendToAdd) && user.Id != friendId)
            {
                user.Friends.Add(friendToAdd);
                var friendsOfUser = user.Friends.Select(friend => friend.Username);
                return Ok(friendsOfUser);
            }
            else if (user.Id == friendId)
            {
                return BadRequest(new { error = "Sorry but you can't be friends with yourself" });
            }
            else
            {
                return BadRequest(new { error = $"The user is already friends with {friendToAdd.Username}" });
            }
        }

        // Remove Friend from User
        [HttpPut("{userId}/removeFriend/{friendId}")]
        public ActionResult RemoveFriend(int userId, int friendId)
        {
            var users = _userRepository.GetAllUsers();
            var user = users.FirstOrDefault(u => u.Id == userId);
            var friendToRemove = users.FirstOrDefault(f => f.Id == friendId);

            if (user.Friends.Contains(friendToRemove))
            {
                user.Friends.Remove(friendToRemove);
                return Ok($"{user.Username} is no longer friends with {friendToRemove.Username}");
            }
            else
            {
                return BadRequest(new { error = $"I'm sorry {user.Username}, but you don't have any friends. You should be nicer to people." });
            }
        }

        // GET User's Friends
        [HttpGet("{userId}/friends")]
        public ActionResult GetFriends(int userId)
        {
            var user = _userRepository.GetSingleUser(userId);
            if (user.Friends.Count == 0) return BadRequest(new { error = $"{user.Username} doesnt have any friends" });

            var friendsOfUser = user.Friends.Select(friend => friend.Username);
            return Ok(friendsOfUser);
        }

        // GET User's Friends of Friends
        [HttpGet("{userId}/friends/friendsOfFriends")]
        public ActionResult GetFriendsOfFriends(int userId)
        {
            var user = _userRepository.GetSingleUser(userId);
            if (user.Friends.Count == 0) return BadRequest(new { error = $"{user.Username} doesnt have any friends" });

            var friendsOfUser = user.Friends
                .SelectMany(friend => friend.Friends)
                .Where(f => f != user).ToList();
            var friends = friendsOfUser.Select(friend => friend.Username).Distinct();
            return Ok(friends);
        }


        // -------------------------------- Services --------------------------------

        //[HttpGet("{id}/service")]
        //public ActionResult ListService(string id)
        //{
        //    var userServiceList = _userRepository.GetSingleUser(id).Services;
        //    return Ok(userServiceList);
        //}

        //[HttpPut("{id}/service/add")]
        //public ActionResult AddService(string id, string service)
        //{
        //    var userServiceList = _userRepository.GetSingleUser(id).Services;

        //    if(!userServiceList.Contains(service))
        //    {
        //        userServiceList.Add(service);
        //        return Ok();
        //    }
        //    else
        //    {
        //        return BadRequest("You can't add the same service twice...");
        //    }
        //}

        //[HttpPut("{id}/service/remove")]
        //public ActionResult RemoveService(string id, string service)
        //{
        //    var userServicesList = _userRepository.GetSingleUser(id).Services;

        //    userServicesList.Remove(service);
        //    return Ok();
        //}

        // -------------------------------- Enemies --------------------------------

        // Add Enemy to User //
        [HttpPut("{userId}/addEnemy/{enemyId}")]
        public ActionResult AddEnemy(int userId, int enemyId)
        {
            var users = _userRepository.GetAllUsers();
            var user = users.First(u => u.Id == userId);
            var enemyToAdd = users.First(f => f.Id == enemyId);

            if (!user.Enemies.Contains(enemyToAdd) && user.Id != enemyId)
            {
                user.Enemies.Add(enemyToAdd);
                return Ok(user);
            }
            else if (userId == enemyId)
            {
                return BadRequest("Are you enemies with yourself?");
            }
            else
            {
                return BadRequest($"You are already enemies with {enemyToAdd.Username}");
            }
        }

        // Get enemy of User //
        [HttpGet("{userId}/enemies")]
        public ActionResult GetEnemies(int userId)
        {
            var inmateEnemies = _userRepository.GetSingleUser(userId);
            return Ok(inmateEnemies.Enemies);
        }

        // Remove enemy //
        [HttpPut("{userId}/removeEnemy/{enemyId}")]
        public ActionResult RemoveEnemy(int userId, int enemyId)
        {
            var users = _userRepository.GetAllUsers();
            var user = users.First(u => u.Id == userId);
            var enemyToRemove = users.First(f => f.Id == enemyId);

            if (user.Enemies.Contains(enemyToRemove))
            {
                user.Enemies.Remove(enemyToRemove);
                return Ok(user);
            }
            else
            {
                return BadRequest("Congratulations! You don't have any enemies...or so you think...so watch your back...");
            }
        }

        //------------------------ Release Date Calculation ----------------------
        
        [HttpGet("{userId}/release")]
        public ActionResult GetDaysTilRelease(int userId)
        {
            var inmate = _userRepository.GetSingleUser(userId);
            var daysTilRelease = inmate.ReleaseDate.Subtract(DateTime.Today).Days;

            return Ok($"{inmate.Username} has {daysTilRelease} days till they are released");
        }

        //------------------------ Warden ----------------------
        [HttpGet("warden")]
        public ActionResult GetAllInmatesForWarden(int userId)
        {
            var allUsers = _userRepository.GetAllUsers();
            var inmates = allUsers.Where(user => user.IsWarden == false).Select(user => user.Username);
            var warden = allUsers.Where(user => user.IsWarden == true).Select(user => user.Username);

            string inmateString = string.Join(", ", inmates);
            string wardenString = string.Join("", warden);

            return Ok($"{inmateString} are the inmates at Warden {wardenString}'s prison.");
        }
    }
}