import { NgClass, NgFor, NgIf } from "@angular/common";
import { Component, ViewChild } from "@angular/core";
import { FontAwesomeModule } from "@fortawesome/angular-fontawesome";
import { faArrowLeft, faTimes, faArrowRotateRight } from "@fortawesome/pro-regular-svg-icons";
import { SlidePanelDirective } from "./slide-panel.directive";
import { DxButtonModule, DxToolbarModule } from "devextreme-angular";
import { icon } from "@fortawesome/fontawesome-svg-core";

@Component({
    selector: 'sh-slide-panel',
    templateUrl: './slide-panel.component.html',
    styleUrls: ['./slide-panel.component.scss'],
    imports: [NgClass, NgFor, NgIf, DxButtonModule, DxToolbarModule, FontAwesomeModule, SlidePanelDirective]
})
export class SlidePanelComponent {
    farArrowLeft = icon(faArrowLeft).html[0];
    farTimes = icon(faTimes).html[0];
    farArrowRotateRight = icon(faArrowRotateRight).html[0];

    @ViewChild(SlidePanelDirective, { static: true }) slideHost!: SlidePanelDirective;

    get isOpen(): boolean {
        if (this.slideHost != null && this.slideHost.components.length > 0) {
            return true;
        }
        return false;
    }

    get showBackButton(): boolean {
        if (this.slideHost != null && this.slideHost.components.length > 1) {
            return true;
        }
        return false;
    }

    get titles(): string[] {
        if (this.slideHost != null) {
            return this.slideHost.components.map((component) => component.title ?? '');
        }
        return [];
    }

    get showOverlay(): boolean {
        if (this.slideHost != null && this.slideHost.components.length > 0) {
            if (this.slideHost.components.length > 1
                || this.slideHost.components[this.slideHost.components.length - 1]?.forceOverlay) {
                return true;
            }
        }
        return false;
    }

    reload() {
        if (this.slideHost != null) {
            this.slideHost.reloadComponent();
        }
    }

    back() {
        if (this.slideHost != null) {
            this.slideHost.removeComponent();
        }
    }

    close() {
        if (this.slideHost != null) {
            this.slideHost.clearComponents();
        }
    }
}