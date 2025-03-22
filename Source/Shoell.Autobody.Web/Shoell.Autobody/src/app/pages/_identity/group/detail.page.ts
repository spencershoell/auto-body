import { NgIf } from "@angular/common";
import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, RouterModule } from "@angular/router";
import { DxLoadIndicatorModule } from "devextreme-angular";
import Guid from "devextreme/core/guid";
import { Context, Group } from "src/app/data";
import { GroupDetailComponent } from "src/app/shared/components/_identity/group/detail.component";

@Component({
    selector: 'sh-group-detail-page',
    templateUrl: 'detail.page.html',
    styleUrls: ['detail.page.scss'],
    imports: [NgIf, RouterModule, DxLoadIndicatorModule, GroupDetailComponent]
})
export class GroupDetailPageComponent implements OnInit {

    id!: Guid;
    model?: Group;

    constructor(private route: ActivatedRoute, private context: Context) { }

    async ngOnInit(): Promise<void> {
        this.id = this.route.snapshot.paramMap.get("id") as Guid;
        await this.refresh();
    }

    async refresh(): Promise<void> {
        this.model = (await this.context.groups.load({ filter: ['id', this.id] }))[0];
    }
}