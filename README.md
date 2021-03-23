# ynab-sms
Send SMS notifications when you reach spending limits.

## Usage
`ynab-sms.exe path/to/config_file.json`

## App Config JSON Format
    {
        "access_token" : "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
        "budget_items_json_file" : "path/to/budget_items.json"
    }

## Budget Items JSON Format
    {
        "entries" : [
            {
                "phone_number" : "5555551234",
                "budgets" : [
                    {
                        "id" : "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX",
                        "budget_items" : [
                            {
                                "category" : "..."
                                "line_item" : "..."
                            },
                            {
                                "category" : "...",
                                "line_item" : "..."
                            }
                        ]
                    }
                ]
            }
        ]
    }

