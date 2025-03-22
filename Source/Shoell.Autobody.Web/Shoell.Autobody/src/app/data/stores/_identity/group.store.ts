import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { plainToInstance } from "class-transformer";
import Guid from "devextreme/core/guid";
import { DataFieldTypes, EntityEndpoint } from "src/app/classes";
import { Group, GroupODataStoreModel } from "src/app/data/models";
import { DataEventService } from "src/app/shared/services";
import { environment } from "src/environments/environment";
import { CoreStore } from "..";
import ODataStore from "devextreme/data/odata/store";

const url: string = `${environment.settings.resources.api.endpoint}${EntityEndpoint.Group}`;

@Injectable({
    providedIn: 'root'
})
export class GroupStore extends CoreStore<Group, Guid> {
    override url = url;
    override recycle: ODataStore<Group, Guid>;

    constructor(
        override dataEventService: DataEventService, 
        override http: HttpClient
    ) {
        let config = {
            url: url,
            errorHandler: (error: any) => {
                console.error(error);
            },
            version: 4,
            key: 'id',
            keyType: DataFieldTypes.Guid,
            fieldTypes: GroupODataStoreModel.getFieldTypes(),
            onLoaded: (data: any[]) => {
                for (let i = 0; i < data.length; i++) {
                    data[i] = plainToInstance(Group, data[i]);
                }
            },
            onModified: () => {
                dataEventService.informGroupChanged();
            },
            onUpdating: (key: any, values: any) => {
                delete values.logs;
                delete values.roleGroups;
                delete values.userGroups;
            }
        };
        super(config);

        config.url = `${url}/Recycle`;
        this.recycle = new ODataStore(config);

        this.archive = this.archive.bind(this);
    }

    protected override informChanged(): void {
        this.dataEventService.informGroupChanged();
    }
}