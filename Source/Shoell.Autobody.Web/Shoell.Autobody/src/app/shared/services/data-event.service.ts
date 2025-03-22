import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class DataEventService {
    // Identity
    private _groupChangedSource = new Subject();
    private _roleChangedSource = new Subject();
    private _userChangedSource = new Subject();
    private _userGroupChangedSource = new Subject();
    private _userRoleChangedSource = new Subject();
    private _roleGroupChangedSource = new Subject();

    // System
    private _logChangedSource = new Subject();

    // Identity
    groupChanged$ = this._groupChangedSource.asObservable();
    roleChanged$ = this._roleChangedSource.asObservable();
    userChanged$ = this._userChangedSource.asObservable();
    userGroupChanged$ = this._userGroupChangedSource.asObservable();
    userRoleChanged$ = this._userRoleChangedSource.asObservable();
    roleGroupChanged$ = this._roleGroupChangedSource.asObservable();

    // System
    logChanged$ = this._logChangedSource.asObservable();

    // Identity
    informGroupChanged() {
        this._groupChangedSource.next(true);
    }

    informRoleChanged() {
        this._roleChangedSource.next(true);
    }

    informUserChanged() {
        this._userChangedSource.next(true);
    }

    informUserGroupChanged() {
        this._userGroupChangedSource.next(true);
    }

    informUserRoleChanged() {
        this._userRoleChangedSource.next(true);
    }

    informRoleGroupChanged() {
        this._roleGroupChangedSource.next(true);
    }

    // System
    informLogChanged() {
        this._logChangedSource.next(true);
    }
}
