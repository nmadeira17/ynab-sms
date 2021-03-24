# ynab-sms
Send SMS notifications when you reach spending limits.

## Usage
`ynab-sms.exe path/to/config_file.json`

## App Config JSON Format
    {
        "access_token" : "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
        "budget_config_json_file" : "path/to/budget_config.json",
        "twilio_sid" : "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
        "twilio_auth_token" : "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
        "twilio_phone_number" : "+15555551234"
    }

## Budget Items JSON Format
    {
        "users" : [
            {
                "phone_number" : "5555551234",
                "budgets" : [
                    {
                        "id" : "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX",
                        "budget_config" : [
                            {
                                "category_group" : "..."
                                "category" : "..."
                            },
                            {
                                "category_group" : "...",
                                "category" : "..."
                            }
                        ]
                    }
                ]
            }
        ]
    }

