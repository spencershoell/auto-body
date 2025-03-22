import ODataContext from "devextreme/data/odata/context";
import { environment } from "src/environments/environment";
import { Injectable } from "@angular/core";
import { GroupStore, Role_GroupStore, RoleStore, User_GroupStore, UserStore } from "./stores";

@Injectable({
    providedIn: 'root'
})
export class Context {
    protected baseUrl: string = environment.settings.resources.api.endpoint;

    odataContext: ODataContext;

    constructor(
        public groups: GroupStore,
        public roles: RoleStore,
        public users: UserStore,

        public role_Groups: Role_GroupStore,
        public user_Groups: User_GroupStore,
    ) {
        this.odataContext = new ODataContext({
            url: this.baseUrl,
            errorHandler: (error) => {
                console.error(error);
            },
            version: 4
        })
    }
}