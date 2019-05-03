using ClinkedIn2.Data;
using ClinkedIn2.Validators;
using Microsoft.AspNetCore.Mvc;

namespace ClinkedIn2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InterestsController : ControllerBase
    {
        readonly UserRepository _userRepository;
        readonly InterestRepository _interestsRepository;
        readonly CreateUserRequestValidator _validator;

        public InterestsController()
        {
            _validator = new CreateUserRequestValidator();
            _userRepository = new UserRepository();
            _interestsRepository = new InterestRepository();
        }

        // -------------------------------- Interests --------------------------------

        [HttpGet]
        public ActionResult GetAllInterests(int userId)
        {
            var userInterestList = _userRepository.GetSingleUser(userId).Interests;
            return Ok(userInterestList);
        }

        [HttpPut("{id}/interest/add")]
        public ActionResult AddInterest(int id, string interest)
        {

            var userInterestList = _userRepository.GetSingleUser(id).Interests;
            if (userInterestList.Contains(interest))
            {
                return BadRequest(new { error = $"I'm sorry but {interest} is alreday in your list" });
            }
            else
            {
                userInterestList.Add(interest);
            }

            return Ok();
        }

        //[HttpPut("{id}/interest/remove")]
        //public ActionResult RemoveInterest(string id, string interest)
        //{
        //    var userInterestList = _userRepository.GetSingleUser(id).Interests;


        //    userInterestList.Remove(interest);
        //    return Ok();

        //}

        //[HttpGet("interest/list")]
        //public ActionResult Interest()
        //{
        //    var userIntrestList = _userRepository.ReadInterestList();
        //    return Ok(userIntrestList);
        //}

        //[HttpGet("interest/common")]
        //public ActionResult CommonInterest(string interest)
        //{
        //    var aUser = _userRepository.GetAllUsers();
        //    string commonInterestusers = "";
        //    //var aUsersInterest = aUser.First(user => user.Interests = interest);
        //    foreach (User individualUser in aUser)
        //    {
        //        string thisTHing = individualUser.ToString();
        //        var thisListInterest = individualUser.Interests;
        //        foreach (string inmateInterest in thisListInterest)
        //        {
        //            if (inmateInterest == interest)
        //            {
        //                commonInterestusers += individualUser.Username + ", ";
        //            }

        //        }

        //    }

        //    return Ok(commonInterestusers);

        //}

    }
}