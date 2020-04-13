﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MilkmenUnion.Controllers.Models;
using Shouldly;
using Xunit;

namespace Milkmenunion.Tests.Controllers
{
    public class EmployeeControllerTests: IAsyncLifetime
    {
        private TestSystem _testSystem;
        private HttpClient _client;

        public async Task InitializeAsync()
        {
            _testSystem = await TestSystem.Create();
            _client = _testSystem.CreateClient();
        }

        public Task DisposeAsync()
        {
            _testSystem.Dispose();
            return Task.CompletedTask;
        }

        [Fact]
        public async Task can_get_all_employees()
        {
            var getResult = await _client.GetMessage<GetAllEmployeesResult>("employee");
            getResult.Total.ShouldBe(The.AllEmployees.Count());
        }
    }
}
