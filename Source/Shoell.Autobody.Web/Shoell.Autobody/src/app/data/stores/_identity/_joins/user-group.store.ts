import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { plainToInstance } from "class-transformer";
import Guid from "devextreme/core/guid";
import { Common, DataFieldTypes, EntityEndpoint } from "src/app/classes";
import { User_Group, User_GroupODataStoreModel } from "src/app/data/models";
import { DataEventService } from "src/app/shared/services";
import { environment } from "src/environments/environment";
import ODataStore from "devextreme/data/odata/store";
import { catchError, lastValueFrom, of } from "rxjs";

const url: string = `${environment.settings.resources.api.endpoint}${EntityEndpoint.User_Group}`;

@Injectable({
    providedIn: 'root'
})
export class User_GroupStore extends ODataStore<User_Group, [Guid, Guid]> {

    constructor(
        protected dataEventService: DataEventService,
        protected http: HttpClient
    ) {
        let config = {
            url: url,
            errorHandler: (error: any) => {
                console.error(error);
            },
            version: 4,
            key: ['userId', 'groupId'],
            keyType: {
                userId: DataFieldTypes.Guid,
                groupId: DataFieldTypes.Guid
            },
            fieldTypes: User_GroupODataStoreModel.getFieldTypes(),
            onLoaded: (data: any[]) => {
                for (let i = 0; i < data.length; i++) {
                    data[i] = plainToInstance(User_Group, data[i]);
                }
            },
            onModified: () => {
                dataEventService.informRoleChanged();
            },
            onUpdating: (key: any, values: any) => {
                delete values.user;
                delete values.group;
            }
        };
        super(config);
    }

    async addToUsers(groupId: Guid, userIds: Guid[]): Promise<void> {
        await lastValueFrom(this.http
            .post(`${url}/AddToUsers`, { model: { groupId: groupId, userIds: userIds } }, {})
            .pipe(
                catchError(error => {
                    if (error instanceof HttpErrorResponse) {
                        Common.toastError(Common.getServerErrorMessage(error.error));
                    } else {
                        Common.toastError(Common.getServerErrorMessage(error.message));
                    }
                    return of();
                })
            ));
    }

    async addToGroups(userId: Guid, groupIds: Guid[]): Promise<void> {
        await lastValueFrom(this.http
            .post(`${url}/AddToGroups`, { model: { userId: userId, groupIds: groupIds } }, {})
            .pipe(
                catchError(error => {
                    if (error instanceof HttpErrorResponse) {
                        Common.toastError(Common.getServerErrorMessage(error.error));
                    } else {
                        Common.toastError(Common.getServerErrorMessage(error.messagex));
                    }
                    return of();
                })
            ));
    }
}