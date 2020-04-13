using System;
using System.Collections.Generic;
using System.Text;
using MilkmenUnion;
using Shouldly;
using Xunit;

namespace Milkmenunion.Tests
{
    public class DateTimeExtensionsTests
    {
        [Fact]
        public void can_calculate_how_old_a_person_is()
        {
            var now = new DateTime(2020,4,1);

            var birthday = new DateTime(1990,1,1);
            birthday.GetAge(now).ShouldBe(30);
        }

        [Fact]
        public void should_count_year_only_after_day_and_month_reached()
        {
            var now = new DateTime(2020, 4, 1);

            var birthday = new DateTime(1990, 4, 10);
            birthday.GetAge(now).ShouldBe(29);
        }
    }
}
