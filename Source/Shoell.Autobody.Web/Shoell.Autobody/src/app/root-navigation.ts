import { faGear } from "@fortawesome/pro-solid-svg-icons";
import { icon } from "@fortawesome/fontawesome-svg-core";

let fasGear = icon(faGear).html[0];

export const navigation: any = [
    {
        text: 'Settings',
        icon: fasGear,
        items: [
            {
                text: 'Users',
                path: '/users'
            },
            {
                text: 'Groups',
                path: '/group'
            }
        ]
    },
];
