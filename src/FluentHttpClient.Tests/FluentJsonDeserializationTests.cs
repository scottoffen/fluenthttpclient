using System.Net;
using System.Text;
using System.Text.Json;

#if NET6_0_OR_GREATER
using System.Text.Json.Nodes;
#endif

namespace FluentHttpClient.Tests;

public class FluentJsonDeserializationTests
{
    public class ReadJsonAsyncTypedTests
    {
        private sealed class SampleModel
        {
            public string? Name { get; set; }
            public int Value { get; set; }
        }

        [Fact]
        public async Task ReadJsonAsync_ReturnsDeserializedObject_WhenContentIsValidJson()
        {
            var json = """{ "Name": "Test", "Value": 42 }""";
            var response = CreateResponse(json);

            var result = await response.ReadJsonAsync<SampleModel>();

            result.ShouldNotBeNull();
            result!.Name.ShouldBe("Test");
            result.Value.ShouldBe(42);
        }

        [Fact]
        public async Task ReadJsonAsync_ThrowsJsonException_WhenContentIsEmpty()
        {
            var response = CreateResponse(string.Empty);
            var token = CancellationToken.None;

            await Should.ThrowAsync<JsonException>(
                async () => await response.ReadJsonAsync<SampleModel>(token));
        }

        [Fact]
        public async Task ReadJsonAsync_UsesCustomSerializerOptions_WhenProvided()
        {
            var json = """{ "name": "Test", "value": 42 }""";
            var response = CreateResponse(json);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var result = await response.ReadJsonAsync<SampleModel>(options);

            result.ShouldNotBeNull();
            result!.Name.ShouldBe("Test");
            result.Value.ShouldBe(42);
        }

        [Fact]
        public async Task ReadJsonAsync_ReturnsDeserializedObject_WhenCalledOnTaskOfResponse()
        {
            var json = """{ "Name": "Task", "Value": 7 }""";
            var response = CreateResponse(json);
            var responseTask = Task.FromResult(response);

            var result = await responseTask.ReadJsonAsync<SampleModel>();

            result.ShouldNotBeNull();
            result!.Name.ShouldBe("Task");
            result.Value.ShouldBe(7);
        }

        [Fact]
        public async Task ReadJsonAsync_ThrowsOperationCanceled_WhenTokenIsCanceled()
        {
            var response = CreateResponse("""{ "Name": "Canceled", "Value": 1 }""");

            using var cts = new CancellationTokenSource();
            cts.Cancel();

            await Should.ThrowAsync<OperationCanceledException>(
                async () => await response.ReadJsonAsync<SampleModel>(cts.Token));
        }

        private static HttpResponseMessage CreateResponse(string? json)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);

            if (json is null)
            {
                response.Content = null;
            }
            else
            {
                response.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            return response;
        }
    }

    public class ReadJsonAsyncOverloadTests
    {
        private sealed class SampleModel
        {
            public string? Name { get; set; }
            public int Value { get; set; }
        }

        [Fact]
        public async Task ReadJsonAsync_TaskOverloads_ProduceSameResult()
        {
            var json = """{ "Name": "Bundle", "Value": 123 }""";

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            using var cts = new CancellationTokenSource();

            var result1 = await Task.FromResult(CreateResponse(json))
                .ReadJsonAsync<SampleModel>();

            var result2 = await Task.FromResult(CreateResponse(json))
                .ReadJsonAsync<SampleModel>(options);

            var result3 = await Task.FromResult(CreateResponse(json))
                .ReadJsonAsync<SampleModel>(cts.Token);

            var result4 = await Task.FromResult(CreateResponse(json))
                .ReadJsonAsync<SampleModel>(options, cts.Token);

            var results = new[] { result1, result2, result3, result4 };

            foreach (var result in results)
            {
                result.ShouldNotBeNull();
                result!.Name.ShouldBe("Bundle");
                result.Value.ShouldBe(123);
            }
        }

        private static HttpResponseMessage CreateResponse(string json)
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }
    }

    public class ReadJsonDocumentAsyncTests
    {
        [Fact]
        public async Task ReadJsonDocumentAsync_ReturnsJsonDocument_WhenContentIsValidJson()
        {
            var json = """{ "name": "doc", "value": 5 }""";
            var response = CreateResponse(json);

            var document = await response.ReadJsonDocumentAsync();

            document.ShouldNotBeNull();
            document!.RootElement.GetProperty("name").GetString().ShouldBe("doc");
            document.RootElement.GetProperty("value").GetInt32().ShouldBe(5);
        }

        [Fact]
        public async Task ReadJsonDocumentAsync_RespectsJsonDocumentOptions_WhenAllowTrailingCommasIsTrue()
        {
            var json = "{ \"name\": \"doc\", }";
            var response = CreateResponse(json);

            var options = new JsonDocumentOptions
            {
                AllowTrailingCommas = true
            };

            var document = await response.ReadJsonDocumentAsync(options);

            document.ShouldNotBeNull();
            document!.RootElement.GetProperty("name").GetString().ShouldBe("doc");
        }

        [Fact]
        public async Task ReadJsonDocumentAsync_ThrowsJsonException_WhenTrailingCommaNotAllowed()
        {
            var json = "{ \"name\": \"doc\", }";
            var response = CreateResponse(json);

            var options = new JsonDocumentOptions
            {
                AllowTrailingCommas = false
            };

            await Should.ThrowAsync<JsonException>(
                async () => await response.ReadJsonDocumentAsync(options));
        }

        [Fact]
        public async Task ReadJsonDocumentAsync_ReturnsJsonDocument_WhenCalledOnTaskOfResponse()
        {
            var json = """{ "name": "task-doc", "value": 9 }""";
            var response = CreateResponse(json);
            var responseTask = Task.FromResult(response);

            var document = await responseTask.ReadJsonDocumentAsync();

            document.ShouldNotBeNull();
            document!.RootElement.GetProperty("name").GetString().ShouldBe("task-doc");
            document.RootElement.GetProperty("value").GetInt32().ShouldBe(9);
        }

        private static HttpResponseMessage CreateResponse(string? json)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);

            if (json is null)
            {
                response.Content = null;
            }
            else
            {
                response.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            return response;
        }
    }

    public class ReadJsonDocumentAsyncOverloadTests
    {
        [Fact]
        public async Task ReadJsonDocumentAsync_HttpResponseMessageOverloads_ProduceSameResult()
        {
            var json = """{ "name": "doc", "value": 5 }""";

            var options = new JsonDocumentOptions
            {
                AllowTrailingCommas = true
            };

            using var cts = new CancellationTokenSource();

            var doc1 = await CreateResponse(json).ReadJsonDocumentAsync();
            var doc2 = await CreateResponse(json).ReadJsonDocumentAsync(options);
            var doc3 = await CreateResponse(json).ReadJsonDocumentAsync(cts.Token);
            var doc4 = await CreateResponse(json).ReadJsonDocumentAsync(options, cts.Token);

            var documents = new[] { doc1, doc2, doc3, doc4 };

            foreach (var document in documents)
            {
                document.ShouldNotBeNull();
                document!.RootElement.GetProperty("name").GetString().ShouldBe("doc");
                document.RootElement.GetProperty("value").GetInt32().ShouldBe(5);
            }
        }

        [Fact]
        public async Task ReadJsonDocumentAsync_TaskOverloads_ProduceSameResult()
        {
            var json = """{ "name": "task-doc", "value": 9 }""";

            var options = new JsonDocumentOptions
            {
                AllowTrailingCommas = true
            };

            using var cts = new CancellationTokenSource();

            var doc1 = await Task.FromResult(CreateResponse(json)).ReadJsonDocumentAsync();
            var doc2 = await Task.FromResult(CreateResponse(json)).ReadJsonDocumentAsync(options);
            var doc3 = await Task.FromResult(CreateResponse(json)).ReadJsonDocumentAsync(cts.Token);
            var doc4 = await Task.FromResult(CreateResponse(json)).ReadJsonDocumentAsync(options, cts.Token);

            var documents = new[] { doc1, doc2, doc3, doc4 };

            foreach (var document in documents)
            {
                document.ShouldNotBeNull();
                document!.RootElement.GetProperty("name").GetString().ShouldBe("task-doc");
                document.RootElement.GetProperty("value").GetInt32().ShouldBe(9);
            }
        }

        private static HttpResponseMessage CreateResponse(string json)
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }
    }

#if NET6_0_OR_GREATER
    public class ReadJsonObjectAsyncTests
    {
        [Fact]
        public async Task ReadJsonObjectAsync_ReturnsJsonObject_WhenContentIsValidJsonObject()
        {
            var json = """{ "name": "obj", "value": 3 }""";
            var response = CreateResponse(json);

            var jsonObject = await response.ReadJsonObjectAsync();

            jsonObject.ShouldNotBeNull();
            jsonObject!["name"]!.GetValue<string>().ShouldBe("obj");
            jsonObject["value"]!.GetValue<int>().ShouldBe(3);
        }

        [Fact]
        public async Task ReadJsonObjectAsync_ReturnsNull_WhenJsonIsNotAnObject()
        {
            var json = """[ 1, 2, 3 ]""";
            var response = CreateResponse(json);

            var jsonObject = await response.ReadJsonObjectAsync();

            jsonObject.ShouldBeNull();
        }

        [Fact]
        public async Task ReadJsonObjectAsync_UsesOptionsAndCancellation_WhenProvided()
        {
            var json = """{ "name": "opt", "value": 10 }""";
            var response = CreateResponse(json);

            var nodeOptions = new JsonNodeOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var documentOptions = new JsonDocumentOptions
            {
                AllowTrailingCommas = true
            };

            using var cts = new CancellationTokenSource();

            var jsonObject = await response.ReadJsonObjectAsync(
                nodeOptions,
                documentOptions,
                cts.Token);

            jsonObject.ShouldNotBeNull();
            jsonObject!["name"]!.GetValue<string>().ShouldBe("opt");
            jsonObject["value"]!.GetValue<int>().ShouldBe(10);
        }

        [Fact]
        public async Task ReadJsonObjectAsync_ReturnsJsonObject_WhenCalledOnTaskOfResponse()
        {
            var json = """{ "name": "task-obj", "value": 11 }""";
            var response = CreateResponse(json);
            var responseTask = Task.FromResult(response);

            var jsonObject = await responseTask.ReadJsonObjectAsync();

            jsonObject.ShouldNotBeNull();
            jsonObject!["name"]!.GetValue<string>().ShouldBe("task-obj");
            jsonObject["value"]!.GetValue<int>().ShouldBe(11);
        }

        private static HttpResponseMessage CreateResponse(string? json)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);

            if (json is null)
            {
                response.Content = null;
            }
            else
            {
                response.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            return response;
        }
    }

    public class ReadJsonObjectAsyncOverloadTests
    {
        [Fact]
        public async Task ReadJsonObjectAsync_HttpResponseMessageOverloads_ProduceSameResult()
        {
            var json = """{ "name": "obj-bundle", "value": 77 }""";

            var nodeOptions = new JsonNodeOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var documentOptions = new JsonDocumentOptions
            {
                AllowTrailingCommas = true
            };

            using var cts = new CancellationTokenSource();

            var obj1 = await CreateResponse(json).ReadJsonObjectAsync();
            var obj2 = await CreateResponse(json).ReadJsonObjectAsync(cts.Token);
            var obj3 = await CreateResponse(json).ReadJsonObjectAsync(nodeOptions);
            var obj4 = await CreateResponse(json).ReadJsonObjectAsync(nodeOptions, cts.Token);
            var obj5 = await CreateResponse(json).ReadJsonObjectAsync(documentOptions);
            var obj6 = await CreateResponse(json).ReadJsonObjectAsync(documentOptions, cts.Token);
            var obj7 = await CreateResponse(json).ReadJsonObjectAsync(nodeOptions, documentOptions);
            var obj8 = await CreateResponse(json).ReadJsonObjectAsync(nodeOptions, documentOptions, cts.Token);

            var objects = new[] { obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8 };

            foreach (var jsonObject in objects)
            {
                jsonObject.ShouldNotBeNull();
                jsonObject!["name"]!.GetValue<string>().ShouldBe("obj-bundle");
                jsonObject["value"]!.GetValue<int>().ShouldBe(77);
            }
        }

        [Fact]
        public async Task ReadJsonObjectAsync_TaskOverloads_ProduceSameResult()
        {
            var json = """{ "name": "task-obj-bundle", "value": 88 }""";

            var nodeOptions = new JsonNodeOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var documentOptions = new JsonDocumentOptions
            {
                AllowTrailingCommas = true
            };

            using var cts = new CancellationTokenSource();

            var obj1 = await Task.FromResult(CreateResponse(json)).ReadJsonObjectAsync();
            var obj2 = await Task.FromResult(CreateResponse(json)).ReadJsonObjectAsync(cts.Token);
            var obj3 = await Task.FromResult(CreateResponse(json)).ReadJsonObjectAsync(nodeOptions);
            var obj4 = await Task.FromResult(CreateResponse(json)).ReadJsonObjectAsync(nodeOptions, cts.Token);
            var obj5 = await Task.FromResult(CreateResponse(json)).ReadJsonObjectAsync(documentOptions);
            var obj6 = await Task.FromResult(CreateResponse(json)).ReadJsonObjectAsync(documentOptions, cts.Token);
            var obj7 = await Task.FromResult(CreateResponse(json)).ReadJsonObjectAsync(nodeOptions, documentOptions);
            var obj8 = await Task.FromResult(CreateResponse(json)).ReadJsonObjectAsync(nodeOptions, documentOptions, cts.Token);

            var objects = new[] { obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8 };

            foreach (var jsonObject in objects)
            {
                jsonObject.ShouldNotBeNull();
                jsonObject!["name"]!.GetValue<string>().ShouldBe("task-obj-bundle");
                jsonObject["value"]!.GetValue<int>().ShouldBe(88);
            }
        }

        private static HttpResponseMessage CreateResponse(string json)
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }
    }
#endif

}
