import { Injectable } from "@angular/core";
import { Subject } from "rxjs";
import { Role, User } from "src/app/data/models";
import { UserStore } from "src/app/data/stores";

@Injectable({
    providedIn: 'root'
})
export class AuthorizationService {
    private _roleSource = new Subject<Role[]>();
    private _userSource = new Subject<User>();

    roles: Role[] = [];
    user: User = new User();

    roles$ = this._roleSource.asObservable();
    user$ = this._userSource.asObservable();

    constructor(private userStore: UserStore) {
        this.refresh();
    }

    async refresh() {
        this.roles = await this.userStore.roles();
        this._roleSource.next(this.roles);

        this.user = await this.userStore.info();
        this._userSource.next(this.user);
    }

    async isInRole(role: string, force: boolean = false) {
        if (force || this.roles.length == 0) {
            await this.refresh();
        }

        return this.roles.find(r => {
            return r.name?.includes(role);
        }) != null;
    }

    hasRole(target: string, operation: string) {
        if (this.roles.length > 0) {
            return this.roles
                .find(r => {
                    return r.target?.includes(target) && r.operation?.includes(operation);
                }) != null;
        }
        return false;
    }

    canCreate(target: string) {
        if (this.roles.length > 0) {
            return this.roles
                .find(r => {
                    return r.target?.includes(target) && r.operation?.includes("Create");
                }) != null;
        }
        return false;
    }

    canRead(target: string) {
        if (this.roles.length > 0) {
            return this.roles
                .find(r => {
                    return r.target?.includes(target) && r.operation?.includes("Read");
                }) != null;
        }
        return false;
    }

    canModify(target: string) {
        if (this.roles.length > 0) {
            return this.roles
                .find(r => {
                    return r.target?.includes(target) && r.operation?.includes("Update");
                }) != null;
        }
        return false;
    }

    canUpdate(target: string) {
        if (this.roles.length > 0) {
            return this.roles
                .find(r => {
                    return r.target?.includes(target) && r.operation?.includes("Update");
                }) != null;
        }
        return false;
    }

    canDelete(target: string) {
        if (this.roles.length > 0) {
            return this.roles
                .find(r => {
                    return r.target?.includes(target) && r.operation?.includes("Delete");
                }) != null;
        }
        return false;
    }

    canArchive(target: string) {
        if (this.roles.length > 0) {
            return this.roles
                .find(r => {
                    return r.target?.includes(target) && r.operation?.includes("Archive");
                }) != null;
        }
        return false;
    }

    canRestore(target: string) {
        if (this.roles.length > 0) {
            return this.roles
                .find(r => {
                    return r.target?.includes(target) && r.operation?.includes("Restore");
                }) != null;
        }
        return false;
    }

    canRecycle(target: string) {
        if (this.roles.length > 0) {
            return this.roles
                .find(r => {
                    return r.target?.includes(target) && r.operation?.includes("Recycle");
                }) != null;
        }
        return false;
    }

    canRecover(target: string) {
        if (this.roles.length > 0) {
            return this.roles
                .find(r => {
                    return r.target?.includes(target) && r.operation?.includes("Recover");
                }) != null;
        }
        return false;
    }

    canPurge(target: string) {
        if (this.roles.length > 0) {
            return this.roles
                .find(r => {
                    return r.target?.includes(target) && r.operation?.includes("Purge");
                }) != null;
        }
        return false;
    }
}