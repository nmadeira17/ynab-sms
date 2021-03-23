# ynab-sms
Send SMS notifications when you reach spending limits.

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

