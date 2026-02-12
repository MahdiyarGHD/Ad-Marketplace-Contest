import { CheckIcon, ChevronRight } from "lucide-react";
import PageHeader, { PageHeaderTitle } from "../../components/PageHeader";
import Transition from "../../components/Transition";
import useCategoryStore from "../../stores/useCategoryStore";
import { memo, useEffect } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { invokeHapticFeedbackImpact } from "../../utils/common";
import { backButton } from "@tma.js/sdk-react";
import useChannelStore from "../../stores/useChannelStore";
import useCampaignStore from "../../stores/useCampaignStore";
import { useShallow } from "zustand/shallow";
import useUIStore from "../../stores/useUIStore";

function SelectCategory({ title }: { title?: string }) {
	const { categories, getCategories } = useCategoryStore();
	const { setDraftChannelCategory } = useChannelStore();
	const { setDraftCampaignCategory, setDraftCampaignPreferredCategory } =
		useCampaignStore();

	const preferredCategoryIds =
		useCampaignStore(
			useShallow(
				(state) => state.draftCampaign?.targeting?.preferred_category_ids,
			),
		) || [];

	const { set, type } = useParams();

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
				break;
			default:
				navigate(`/category/${category.id}?type=${type}`);
				break;
		}
	};

	useEffect(() => {
		if (!categories?.length) {
			getCategories();
		}
	}, []);

	useEffect(() => {
		backButton.show();
		backButton.onClick(onBackButton);

		if (set === "campaign-preferred") {
			useUIStore.setState({
				mainButton: {
					text: "Retry",
					onClick: onBackButton,
				},
			});
		}

		invokeHapticFeedbackImpact("medium");

		return () => {
			backButton.hide();
			backButton.offClick(onBackButton);
		};
	}, [set, navigate, invokeHapticFeedbackImpact]);

	useEffect(() => {
		if (set === "campaign-preferred")
			useUIStore.setState({
				mainButton: {
					text:
						preferredCategoryIds.length > 0
							? `Select ${preferredCategoryIds.length} Categories`
							: "Done",
					onClick: onBackButton,
				},
			});
	}, [preferredCategoryIds]);

	return (
		<div className="SelectCategory scrollable">
			<PageHeader>
				<PageHeaderTitle>{title || "Select Category"}</PageHeaderTitle>
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
								{set === "campaign-preferred" ? (
									preferredCategoryIds.includes(category.id) && <CheckIcon />
								) : (
									<ChevronRight />
								)}
							</div>
						</div>
					))}
				</Transition>
			</div>
		</div>
	);
}

export default memo(SelectCategory);
