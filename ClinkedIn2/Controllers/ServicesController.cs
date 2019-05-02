﻿using System.Collections.Generic;
using ClinkedIn2.Data;
using ClinkedIn2.Models;
using ClinkedIn2.Validators;
using Microsoft.AspNetCore.Mvc;

namespace ClinkedIn2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        readonly UserRepository _userRepository;
        readonly ServiceRepository _servicesRepository;
        readonly CreateUserRequestValidator _validator;

        public ServicesController()
        {
            _validator = new CreateUserRequestValidator();
            _userRepository = new UserRepository();
            _servicesRepository = new ServiceRepository();
        }

        [HttpGet]
        public ActionResult <List<Services>> GetAllServices()
        {
            var serviceList = _servicesRepository.GetAllServices();
            return Ok(serviceList);
        }

        [HttpPost("{id}/add")]
        public ActionResult ListService(CreateServiceRequest createRequest)
        {
            var serviceList = _servicesRepository.AddService(createRequest.Name, createRequest.Description, createRequest.Price);
            return Ok(serviceList);
        }
    }
}