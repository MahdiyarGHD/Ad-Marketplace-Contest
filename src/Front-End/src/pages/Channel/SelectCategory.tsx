import { ChevronRight } from "lucide-react";
import PageHeader, { PageHeaderTitle } from "../../components/PageHeader";
import Transition from "../../components/Transition";
import useCategoryStore from "../../stores/useCategoryStore";
import { memo, useEffect } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { invokeHapticFeedbackImpact } from "../../utils/common";
import { backButton } from "@tma.js/sdk-react";
import useChannelStore from "../../stores/useChannelStore";
import useCampaignStore from "../../stores/useCampaignStore";

function SelectCategory() {
	const { categories } = useCategoryStore();
	const { setDraftChannelCategory } = useChannelStore();
	const { setDraftCampaignCategory, setDraftCampaignPreferredCategory } =
		useCampaignStore();

	const { set } = useParams();

	const navigate = useNavigate();

	const onBackButton = () => {
		window.history.back();
	};

	const onSelectCategory = (category: any) => {
		switch (set) {
			case "channel":
				setDraftChannelCategory(category);
				navigate("/set-channel-data");
				break;
			case "campaign":
				setDraftCampaignCategory(category);
				navigate("/add-campaign");
				break;
			case "campaign-preferred":
				setDraftCampaignPreferredCategory(category);
				navigate("/add-campaign");
				break;
		}
	};

	useEffect(() => {
		backButton.show();

		backButton.onClick(onBackButton);

		invokeHapticFeedbackImpact("medium");

		return () => {
			backButton.hide();

			backButton.offClick(onBackButton);
		};
	}, []);

	return (
		<div className="SelectCategory scrollable">
			<PageHeader>
				<PageHeaderTitle>Select Category</PageHeaderTitle>
			</PageHeader>

			<div className="CategoryList Items">
				<Transition state eachElement eachElementDelay={20}>
					{categories.map((category) => (
						<div
							key={category.id}
							className="Item"
							onClick={() => onSelectCategory(category)}
						>
							<div className="icon">{category.icon}</div>
							<div className="body">
								<div className="title">{category.name}</div>
								<div className="subtitle">{category.description}</div>
							</div>
							<div className="meta">
								<ChevronRight />
							</div>
						</div>
					))}
				</Transition>
			</div>
		</div>
	);
}

export default memo(SelectCategory);
