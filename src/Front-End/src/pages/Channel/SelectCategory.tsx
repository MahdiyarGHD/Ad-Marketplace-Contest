import { ChevronRight } from "lucide-react";
import PageHeader, { PageHeaderTitle } from "../../components/PageHeader";
import Transition from "../../components/Transition";
import useCategoryStore from "../../stores/useCategoryStore";
import { memo, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { invokeHapticFeedbackImpact } from "../../utils/common";
import { backButton } from "@tma.js/sdk-react";
import useChannelStore from "../../stores/useChannelStore";

function SelectCategory() {
	const { categories } = useCategoryStore();
	const { setDraftChannelCategory } = useChannelStore();

	const navigate = useNavigate();

	const onBackButton = () => {
		navigate("/set-channel-data");
	};

	const onSelectCategory = (category: any) => {
		setDraftChannelCategory(category);
		navigate("/set-channel-data");
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
