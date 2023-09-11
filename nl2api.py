import gradio as gr
import os
import json
import openai
import gradio as gr
from api_mappings import MAPPER

# Get the value of the openai_api_key from environment variable
openai.api_key = os.environ.get("OPENAI_API_KEY")
KEY = os.environ.get("VIRUSTOTAL_API_KEY")
print(KEY)
from base64 import urlsafe_b64encode
import os.path

FUNCTIONS = [x[0] for x in MAPPER.values()]

def run_functioncalling(messages):
    # STEP1: Get user Input to Model
    response = openai.ChatCompletion.create(
        model="gpt-3.5-turbo-0613",
        messages=messages,
        functions=FUNCTIONS,
        function_call="auto",
    )
    message = response["choices"][0]["message"]
    print(message)
    #function calling
    if message.get("function_call"):
        function_name = message["function_call"]["name"]
        arguments = json.loads(message["function_call"]["arguments"])
        if function_name in MAPPER:
            print("The selected function is "+ function_name)
            f_name = MAPPER[function_name][1]
            f_args = [arguments.get(x) for x in MAPPER[function_name][0]["parameters"]["required"]]
            function_response = list(map(f_name, f_args))[0]
        else:
            return "No function that can solve this query. Please try a different question."
        
        return function_response
    else:
        return message

def predict(inputs, chatbot):

    messages = []
    for user, assistant in chatbot:
        messages.append({"role": "user", "content":user })
        messages.append({"role": "assistant", "content":assistant})
    messages.append({"role": "user", "content": inputs})
    messages.append({"role": "assistant", "content": str(run_functioncalling(messages))[:30000]})
    messages.append({"role": "user", "content": "Summarize the above response based on the question I asked earlier."})
    print(messages)
    # a ChatCompletion request summarization
    response = openai.ChatCompletion.create(
        model='gpt-4-0314',
        messages= messages, # example :  [{'role': 'user', 'content': "What is life? Answer in three words."}],
        temperature=1.0,
        stream=True  # for streaming the output to chatbot
    )

    partial_message = ""
    for chunk in response:
        if len(chunk['choices'][0]['delta']) != 0:
          partial_message = partial_message + chunk['choices'][0]['delta']['content']
          yield partial_message 

gr.ChatInterface(predict).queue().launch()