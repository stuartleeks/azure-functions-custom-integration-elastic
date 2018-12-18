import logging

import azure.functions as func


class Wibble:
    def __init__ (self, name, value, foo):
        self.name = name
        self.value = value
        self.foo = foo

def main(req: func.HttpRequest,
         elasticMessage: func.Out[Wibble]) -> func.HttpResponse:
    logging.info('Python HTTP trigger function processed a request.')

    name = req.params.get('name')
    if not name:
        try:
            req_body = req.get_json()
        except ValueError:
            pass
        else:
            name = req_body.get('name')

    if name:
        wibble = Wibble(name, 1, "bar")
        elasticMessage.set(wibble)
        return func.HttpResponse(f"Hello {name}!")
    else:
        return func.HttpResponse(
             "Please pass a name on the query string or in the request body",
             status_code=400
        )


