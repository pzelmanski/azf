The Best of Chuck

The goal for the exercise it to present coding style, compliance with SOLID principles as well as
knowledge of commonly used libraries in .NET framework.
In this task we want to collect resource from external provider.

Requirements:
1. Collect data and store it in SQLite.
2. Implement time triggered Azure function which will be triggered every configurable X

minutes and pull X jokes from external service: https://rapidapi.com/matchilling/api/chuck-
norris/

3. Function should filter out jokes that have more than 200 characters. Database should not
contain any duplicated quotes.
4. Application should allow to easily change jokes provider to other API.
5. Configure logger, error handler and cover the important parts with unit tests
6. Remember about SOLID principles


---


# TimerTrigger - C<span>#</span>

The `TimerTrigger` makes it incredibly easy to have your functions executed on a schedule. This sample demonstrates a simple use case of calling your function every 5 minutes.

## How it works

For a `TimerTrigger` to work, you provide a schedule in the form of a [cron expression](https://en.wikipedia.org/wiki/Cron#CRON_expression)(See the link for full details). A cron expression is a string with 6 separate expressions which represent a given schedule via patterns. The pattern we use to represent every 5 minutes is `0 */5 * * * *`. This, in plain text, means: "When seconds is equal to 0, minutes is divisible by 5, for any hour, day of the month, month, day of the week, or year".

## Learn more


<TODO> Documentation

