import requests
import virustotal_python
from base64 import urlsafe_b64encode
from pprint import pprint

GRAPH_ENDPOINT = 'https://graph.microsoft.com/v1.0/'

def scan_url(url:str)-> str: 
    """Send a file for analysis 

    Args:
        URL (str): The URL that needs scanning

    Returns:
        str: a json string that contains details of whether a URL is malicious or not
    """
    with virustotal_python.Virustotal(KEY) as vtotal:
        try:
            #resp = vtotal.request("urls", data={"url": url}, method="POST")
            # Safe encode URL in base64 format
            # https://developers.virustotal.com/reference/url
            url_id = urlsafe_b64encode(url.encode()).decode().strip("=")
            report = vtotal.request(f"urls/{url_id}")
            return(report.data)
        except virustotal_python.VirustotalError as err:
            print(f"Failed to send URL: {url} for analysis and get the report: {err}")

def check_domain(domain:str)->str:
    """Get information about a domain

    Args:
        domain (str): domain for which information is needed

    Returns:
        str: _description_
    """
    with virustotal_python.Virustotal(KEY) as vtotal:
        resp = vtotal.request(f"domains/{domain}")
        return(resp.data)

def get_groups(token:str) -> str:
    """ List all groups under my organization

    Args:
        token (str): access token for Microsoft Graph API

    Returns:
        str: a json string that contains details of all groups under my organization
    """
    print(f"using Token: {token}----------------------")
    headers = {
        'Authorization': f'Bearer {token}',
        'Content-Type': 'application/json'
    }

    response = requests.get(GRAPH_ENDPOINT + 'groups', headers=headers)
    print(response.json())
    return response.json()


MAPPER = {
    # --------------- groups -----------------------
    "get_groups":[{
        "name": "get_groups",
        "description": "List all groups under my organization",
        "parameters": {
            "type": "object",
            "properties": {
                "token": {"type": "string"}
            },
            "required": ["token"],
        },
    }, get_groups],

    # ---------------- virustotal ----------------------
    "scan_url":[{
        "name": "scan_url",
        "description": "Scan a URL for analysis ",
        "parameters": {
            "type": "object",
            "properties": {
                "url": {"type": "string", "format": "date"}
            },
            "required": ["url"],
        },
    }, scan_url],
    "check_domain":[{
        "name": "check_domain",
        "description": "Get information about a domain",
        "parameters": {
            "type": "object",
            "properties": {
                "domain": {"type": "string"},
            },
            "required": ["domain"],
        },
    }, check_domain]
}

if __name__ == '__main__':
    # Set up the Azure AD app registration details
    CLIENT_ID = ''
    TENANT_ID = ''
    AUTHORITY = f"https://login.microsoftonline.com/{TENANT_ID}"
    TOKEN = ''

    # app = msal.PublicClientApplication(CLIENT_ID, authority=AUTHORITY)
    # device_flow = app.initiate_device_flow(scopes=["User.Read"])
    # print(device_flow['message'])
    # result = app.acquire_token_by_device_flow(device_flow)
    response = get_groups(TOKEN)
    print(response)