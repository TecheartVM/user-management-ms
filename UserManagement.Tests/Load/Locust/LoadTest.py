from locust import HttpUser, TaskSet, task
import uuid
import random
from locust import events

baseUrl = "/api/v1/"

# locust -f LoadTest.py --host=https://localhost:5048


# @events.request.add_listener
# def on_failure(request_type, name, response_time, response_length, response,
#                        context, exception, start_time, url, **kwargs):
#     if exception:
#         print(exception.request.url)
#         print(exception.request.body)
#         print(exception.response.status_code)
#         print(exception.response.content)
#     else:
#         print(url)
#         print(response.status_code)
#         print(response.content)


def generate_uuid():
    return str(uuid.uuid4())


def generate_phone():
    return "+380" + str(random.randint(100000000, 999999999))


def api_url(endpoint):
    return baseUrl + endpoint


class UserBehavior(TaskSet):
    id = ""
    password = "12345678"

    apiKey = ""

    def reg_and_log(self):
        # register
        data = {
            "id": self.id,
            "name": self.id,
            "email": self.id + "@gl.net",
            "password": self.password,
            "phone": generate_phone()
        }
        self.client.post(
            url=api_url("Users/register"),
            headers={"Content-Type": "application/json"},
            json=data)

        # login
        with self.client.post(
                url=api_url("Auth"),
                headers={"Content-Type": "application/json"},
                json={"id": self.id, "name": "", "password": self.password},
                catch_response=True
        ) as response:
            if response.status_code == 200:
                self.apiKey = response.text

    def delete_self(self):
        # delete profile
        self.client.delete(
            url=api_url("Users/" + self.id),
            headers={"authorization": "Bearer " + self.apiKey}
        )

    def on_start(self):
        self.client.verify = False

        self.id = generate_uuid()

        self.reg_and_log()

    def on_stop(self):
        self.delete_self()

    @task(2)
    def get_self(self):
        self.client.get(api_url("Users/" + "3fa85f64-5717-4562-b3fc-2c963f66afa6")) # self.id))

    @task(1)
    def get_all(self):
        self.client.get(api_url("Users"))

    @task(1)
    def edit_profile(self):
        data = {
            "id": self.id,
            "name": self.id,
            "email": self.id + "@gl.net",
            "password": self.password,
            "phone": generate_phone()
        }
        self.client.put(
            api_url("Users"),
            headers={
                "Content-Type": "application/json",
                "authorization": "Bearer " + self.apiKey
            },
            json=data)


class WebsiteUser(HttpUser):
    tasks = {UserBehavior: 1}
    min_wait = 5000
    max_wait = 9000
