from locust import FastHttpUser, TaskSet, task, LoadTestShape

baseUrl = "/api/v1/"


def api_url(endpoint):
    return baseUrl + endpoint


class UserBehavior(TaskSet):
    def on_start(self):
        self.client.verify = False

    @task
    def get_self(self):
        self.client.get(api_url("Users/3fa85f64-5717-4562-b3fc-2c963f66afa6"))


class StagesShape(LoadTestShape):
    stages = [
        {"duration": 5, "users": 25, "spawn_rate": 10},
        {"duration": 6, "users": 200, "spawn_rate": 200},
        {"duration": 20, "users": 10, "spawn_rate": 20},
        {"duration": 21, "users": 200, "spawn_rate": 200},
        {"duration": 36, "users": 35, "spawn_rate": 50}
    ]

    def tick(self):
        run_time = self.get_run_time()

        for stage in self.stages:
            if run_time < stage["duration"]:
                tick_data = (stage["users"], stage["spawn_rate"])
                return tick_data

        return None


class WebsiteUser(FastHttpUser):
    min_wait = 100
    max_wait = 100

    tasks = [UserBehavior]
